using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetAllEmployees()
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Employees", sqlConnection);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        public int GetEmployeesCount()
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = "SELECT COUNT(*) FROM Employees ";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlConnection.Open();
                int employeeCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return employeeCount;
            }
        }

        public DataTable GetEmployeeDetailById(int employeeId)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Employees WHERE Id = @employeeId", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@employeeId", employeeId);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataTable GetEmployeesDetailByGenderBySalary(string gender, int salary)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new(@"SELECT * FROM Employees WHERE Gender = @gender 
                                                  AND Salary > @salary", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@gender", gender);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@salary", salary);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataTable GetEmployeesBySalaryRange(int minimumSalary, int maximumSalary)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new(@" SELECT * FROM Employees 
                                                      WHERE Salary BETWEEN @minimumSalary
                                                      AND @maximumSalary
                                                      ORDER BY Salary", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@minimumSalary", minimumSalary);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@maximumSalary", maximumSalary);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        public string GetEmployeeFullNameById(int employeeId)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {

                string sqlQuery = "SELECT FullName FROM Employees WHERE Id = @employeeId";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@employeeId", employeeId);
                sqlConnection.Open();
                string employeeFullName = Convert.ToString(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return employeeFullName;
            }
        }

        public int Add(EmployeeDto employee)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = @"INSERT INTO Employees(FullName, Email, Gender, DateOfJoining, Salary)
                                            VALUES (@FullName, @Email, @Gender, @DateOfJoining, @Salary)
                                            Select Scope_Identity() ";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@FullName", employee.FullName);
                sqlCommand.Parameters.AddWithValue("@Email", employee.Email);
                sqlCommand.Parameters.AddWithValue("@Gender", employee.Gender);
                sqlCommand.Parameters.AddWithValue("@DateOfJoining", employee.DateOfJoining);
                sqlCommand.Parameters.AddWithValue("@Salary", employee.Salary);
                sqlConnection.Open();
                employee.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return employee.Id;
            }
        }

        public void Update(EmployeeDto employee)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = @"UPDATE Employees SET FullName = @FullName, Email =  @Email, Gender = @Gender,
                                    DateOfJoining = @DateOfJoining, Salary = @Salary
                                    WHERE Id = @Id ";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Id", employee.Id);
                sqlCommand.Parameters.AddWithValue("@FullName", employee.FullName);
                sqlCommand.Parameters.AddWithValue("@Email", employee.Email);
                sqlCommand.Parameters.AddWithValue("@Gender", employee.Gender);
                sqlCommand.Parameters.AddWithValue("@DateOfJoining", employee.DateOfJoining);
                sqlCommand.Parameters.AddWithValue("@Salary", employee.Salary);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }
    }
}
