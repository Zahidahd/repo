using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;

        public CustomerRepository(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new(_Configuration.GetConnectionString("ECommerceDBConnection").ToString());
        }

        public DataTable GetAllCustomers()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Customers", sqlConnection);
            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetCustomerDetailById(int customerId)
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Customers WHERE Id = @customerId", sqlConnection);
            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@customerId", customerId);
            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);
            return dataTable;
        }

        public int GetCustomersCount()
        {
            string sqlQuery = "SELECT COUNT(*) FROM Customers ";
            SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
            sqlConnection.Open();
            int customerCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            sqlConnection.Close();
            return customerCount;
        }     
    }
}
