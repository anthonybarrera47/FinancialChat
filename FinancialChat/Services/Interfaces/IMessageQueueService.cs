namespace FinancialChat.Services.Interfaces
{
    public interface IMessageQueueService
    {
        void Publish(string message);
        void Subscribe(Func<string, Task> onMessageReceived);
    }
}
