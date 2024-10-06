namespace ApiWeb.Services.Interfaces
{
    public interface IRabbitMqService
    {
        void PublishReminderAdded(object reminderMessage);

        void SubscribeToReminderQueue();
    }
}