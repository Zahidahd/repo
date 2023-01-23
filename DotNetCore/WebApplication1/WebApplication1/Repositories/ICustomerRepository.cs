using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public interface ICustomerRepository
    {
        public DataTable GetAllCustomers();
        public DataTable GetCustomerDetailById(int customerId);
        public int GetCustomersCount();
    }
}
        