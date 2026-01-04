namespace PetStuff.Order.Application.Interfaces
{
    public interface IEventBus
    {
        void PublishOrderStatusChangedEvent(int orderId, string userId, string status, string? email = null);
    }
}

