using ApiWeb.Hubs;
using ApiWeb.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using ApiWeb.Models;

public class RabbitMqService : IRabbitMqService
{
    private readonly ConnectionFactory _factory;
    private readonly ILogger<RabbitMqService> _logger;
    private readonly IHubContext<LembreteHub> _hubContext;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMqService(ILogger<RabbitMqService> logger, IHubContext<LembreteHub> hubContext)
    {
        _factory = new ConnectionFactory() { HostName = "localhost" };
        _logger = logger;
        _hubContext = hubContext;

        InitializeRabbitMq();
    }

    private void InitializeRabbitMq()
    {
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "filaLembretes",
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        SubscribeToReminderQueue();
    }

    public void PublishReminderAdded(object reminder)
    {
        var reminderMessage = JsonSerializer.Serialize(reminder);
        try
        {
           
            var body = Encoding.UTF8.GetBytes(reminderMessage);

            _channel.BasicPublish(exchange: "",
                                   routingKey: "filaLembretes",
                                   basicProperties: null,
                                   body: body);

            _logger.LogInformation("Mensagem publicada na fila: {message}", reminderMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem na fila: {message}", reminderMessage);
        }
    }


    public void SubscribeToReminderQueue()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Mensagem recebida da fila: {message}", message);

            try
            {
                var lembrete = JsonSerializer.Deserialize<Lembrete>(message);
                await _hubContext.Clients.All.SendAsync("ReceiveReminder", lembrete);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar mensagem para os clientes: {message}", message);
                _channel.BasicNack(ea.DeliveryTag, false, true); 
            }
        };

        _channel.BasicConsume(queue: "filaLembretes", autoAck: false, consumer: consumer);
    }


    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
