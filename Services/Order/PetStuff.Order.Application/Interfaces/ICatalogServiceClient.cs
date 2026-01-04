namespace PetStuff.Order.Application.Interfaces
{
    public interface ICatalogServiceClient
    {
        Task<bool> CheckStockAsync(List<OrderItemStockRequest> items, string token);
        Task<bool> ReduceStockAsync(List<OrderItemStockRequest> items, string token);
    }

    public class OrderItemStockRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
