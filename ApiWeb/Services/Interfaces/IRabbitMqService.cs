namespace ApiWeb.Services.Interfaces
{
    public interface IRabbitMqService
    {
        void PublishReminderAdded(string message);
        void SubscribeToReminderQueue();
    }
}