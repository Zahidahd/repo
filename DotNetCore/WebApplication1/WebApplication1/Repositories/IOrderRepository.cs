using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public interface IOrderRepository
    {
        public List<OrderDto> GetAllOrdersAsList();
        public int GetOrdersCount();
        public OrderDto GetOrderDetailById(int orderId);
        public List<OrderDto> GetOrdersByCustomerId(int customerId);
        public int Add(OrderDto order);
        public void Update(OrderDto order);
    }
}
