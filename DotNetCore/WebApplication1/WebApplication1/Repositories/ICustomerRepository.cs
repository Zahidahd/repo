using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public interface ICustomerRepository
    {
        public List<CustomerDto> GetAllCustomersAsList();
        public CustomerDto GetCustomerDetailById(int customerId);
        public int GetCustomersCount();
        public List<CustomerDto> GetCustomersDetailByGenderByCountry(string gender, string country);
        public int Add(CustomerDto customer);
        public void Update(CustomerDto customer);
        public string GetCustomerFullNameById(int customerId);
        public int GetEmailCount(CustomerDto customer);
        public int GetMobileNumberCount(CustomerDto customer);
    }
}
