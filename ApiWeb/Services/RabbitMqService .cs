using ApiWeb.Hubs;
using ApiWeb.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

public class RabbitMqService : IRabbitMqService, IDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly ILogger<RabbitMqService> _logger;
    private readonly IHubContext<LembreteHub> _hubContext;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqService(ILogger<RabbitMqService> logger, IHubContext<LembreteHub> hubContext)
    {
        _factory = new ConnectionFactory() { HostName = "localhost" };
        _logger = logger;
        _hubContext = hubContext;
        InitializeRabbitMq();
    }

    private void InitializeRabbitMq()
    {
        try
        {
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "filaLembretes",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            SubscribeToReminderQueue();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar RabbitMQ");
        }
    }

    public void PublishReminderAdded(string message)
    {
        if (_channel == null) return;

        try
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: "",
                routingKey: "filaLembretes",
                basicProperties: null,
                body: body);

            _logger.LogInformation("Mensagem publicada: {Message}", message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao publicar mensagem: {ex.Message}");
        }
    }

    public void SubscribeToReminderQueue()
    {
        if (_channel == null) return;

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(
            queue: "filaLembretes",
            autoAck: false,
            consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}