using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetAllCustomers()
        {
            //Apporach #1 - Recommended
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Customers", sqlConnection);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }

                //Apporach #2
                //SqlConnection sqlConnection = new(_connectionString);
                //SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Customers", sqlConnection);
                //DataTable dataTable = new();
                //sqlDataAdapter.Fill(dataTable);
               //return dataTable;
        }

        public DataTable GetCustomerDetailById(int customerId)
        {
            //Apporach #1 - Recommended
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Customers WHERE Id = @customerId", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@customerId", customerId);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }

            ////Apporach #2
            //SqlConnection sqlConnection = new(_connectionString);
            //SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Customers WHERE Id = @customerId", sqlConnection);
            //sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@customerId", customerId);
            //DataTable dataTable = new();
            //sqlDataAdapter.Fill(dataTable);
            //return dataTable;
        }

        public int GetCustomersCount()
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = "SELECT COUNT(*) FROM Customers ";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlConnection.Open();
                int customerCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return customerCount;
            }
        }

        public DataTable GetCustomersDetailByGenderByCountry(string gender, string country)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new(@"SELECT * FROM Customers WHERE Gender = @gender
                                                  AND Country = @country ", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@gender", gender);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@country", country);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        public int Add(CustomerDto customer)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = @"INSERT INTO Customers(Name, Gender, Age, Country)
                                VALUES (@FullName, @Gender, @Age, @Country)
                                Select Scope_Identity()";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@FullName", customer.FullName);
                sqlCommand.Parameters.AddWithValue("@Gender", customer.Gender);
                sqlCommand.Parameters.AddWithValue("@Age", customer.Age);
                sqlCommand.Parameters.AddWithValue("@Country", customer.Country);
                sqlConnection.Open();
                customer.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return customer.Id;
            }
        }

        public void Update(CustomerDto customer)
        {
            //Approach #1  - Recommended
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = @" UPDATE Customers SET Name = @FullName, Gender = @Gender,
                                         Age = @Age, Country = @Country
                                         WHERE Id = @Id ";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Id", customer.Id);
                sqlCommand.Parameters.AddWithValue("@FullName", customer.FullName);
                sqlCommand.Parameters.AddWithValue("@Gender", customer.Gender);
                sqlCommand.Parameters.AddWithValue("@Age", customer.Age);
                sqlCommand.Parameters.AddWithValue("@Country", customer.Country);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
            ////Approach #2
            //SqlConnection sqlConnection = new(_connectionString);
            //string sqlQuery = @" UPDATE Customers SET Name = @FullName, Gender = @Gender,
            //                         Age = @Age, Country = @Country
            //                         WHERE Id = @Id ";
            //SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
            //sqlCommand.Parameters.AddWithValue("@Id", customer.Id);
            //sqlCommand.Parameters.AddWithValue("@FullName", customer.FullName);
            //sqlCommand.Parameters.AddWithValue("@Gender", customer.Gender);
            //sqlCommand.Parameters.AddWithValue("@Age", customer.Age);
            //sqlCommand.Parameters.AddWithValue("@Country", customer.Country);
            //sqlConnection.Open();
            //sqlCommand.ExecuteNonQuery();
            //sqlConnection.Close();
        }

        public string GetCustomerFullNameById(int customerId)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = "SELECT Name FROM Customers WHERE Id = @customerId";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@customerId", customerId);
                sqlConnection.Open();
                string customerFullName = Convert.ToString(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return customerFullName;
            }
        }
    }
}
