using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;
using WebApplication1.DTO.InputDTO;
using WebApplication1.Enums;

namespace WebApplication1.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<EmployeeDto> GetAllEmployeesAsList()
        {
            List<EmployeeDto> employees = new();

            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Employees", sqlConnection);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    // Approach #1
                    EmployeeDto employeeDto = new();
                    employeeDto.Id = (int)dataTable.Rows[i]["Id"];
                    employeeDto.FullName = (string)dataTable.Rows[i]["FullName"];
                    employeeDto.Gender = (GenderTypes)dataTable.Rows[i]["Gender"];
                    employeeDto.Email = (string)dataTable.Rows[i]["Email"];
                    employeeDto.MobileNumber = (string)dataTable.Rows[i]["MobileNumber"];
                    employeeDto.Salary = (decimal)dataTable.Rows[i]["Salary"];

                    // Approach #2 - Using Object Initializer
                    //EmployeeDto employeeDto = new()
                    //{
                    //    Id = (int)dataTable.Rows[i]["Id"],
                    //    FullName = (string)dataTable.Rows[i]["FullName"],
                    //    Gender = (GenderTypes)dataTable.Rows[i]["Gender"],
                    //    Email = (string)dataTable.Rows[i]["Email"],
                    //    MobileNumber = (string)dataTable.Rows[i]["MobileNumber"],
                    //    Salary = (int)dataTable.Rows[i]["Salary"]
                    //};

                    employees.Add(employeeDto);
                }
                return employees;
            }
        }

        public EmployeeDto GetEmployeeDetailById(int id)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new($"SELECT * FROM Employees Where Id = @id", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@Id", id);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    EmployeeDto employeeDto = new();
                    employeeDto.Id = (int)dataTable.Rows[0]["Id"];
                    employeeDto.FullName = (string)dataTable.Rows[0]["FullName"];
                    employeeDto.Gender = (GenderTypes)dataTable.Rows[0]["Gender"];
                    employeeDto.Email = (string)dataTable.Rows[0]["Email"];
                    employeeDto.MobileNumber = (string)dataTable.Rows[0]["MobileNumber"];
                    employeeDto.Salary = (decimal)dataTable.Rows[0]["Salary"];
                    return employeeDto;
                }
                else              
                    return null;              
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

        public List<EmployeeDto> GetEmployeesDetailByGenderBySalary(int gender, int salary)
        {
            List<EmployeeDto> employees = new();

            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new(@"SELECT * FROM Employees WHERE Gender = @gender 
                                                  AND Salary > @salary", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@gender", gender);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@salary", salary);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    EmployeeDto employeeDto = new();
                    employeeDto.Id = (int)dataTable.Rows[i]["Id"];
                    employeeDto.FullName = (string)dataTable.Rows[i]["FullName"];
                    employeeDto.Gender = (GenderTypes)dataTable.Rows[i]["Gender"];
                    employeeDto.Email = (string)dataTable.Rows[i]["Email"];
                    employeeDto.MobileNumber = (string)dataTable.Rows[i]["MobileNumber"];
                    employeeDto.Salary = (decimal)dataTable.Rows[i]["Salary"];
                    employees.Add(employeeDto);
                }
                return employees;
            }
        }

        public List<EmployeeDto> GetEmployeesBySalaryRange(int minimumSalary, int maximumSalary)
        {
            List<EmployeeDto> employees = new();

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

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    // Approach #1
                    EmployeeDto employeeDto = new();
                    employeeDto.Id = (int)dataTable.Rows[i]["Id"];
                    employeeDto.FullName = (string)dataTable.Rows[i]["FullName"];
                    employeeDto.Gender = (GenderTypes)dataTable.Rows[i]["Gender"];
                    employeeDto.Email = (string)dataTable.Rows[i]["Email"];
                    employeeDto.MobileNumber = (string)dataTable.Rows[i]["MobileNumber"];
                    employeeDto.Salary = (decimal)dataTable.Rows[i]["Salary"];
                    employees.Add(employeeDto);
                }
                return employees;
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
                string sqlQuery = @"INSERT INTO Employees(FullName, Gender, Email, MobileNumber, DateOfJoining, Salary)
                          VALUES (@FullName, @Gender,  @Email, @MobileNumber, @DateOfJoining, @Salary)
                          Select Scope_Identity() ";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@FullName", employee.FullName);
                sqlCommand.Parameters.AddWithValue("@Gender", employee.Gender);
                sqlCommand.Parameters.AddWithValue("@Email", employee.Email);
                sqlCommand.Parameters.AddWithValue("@MobileNumber", employee.MobileNumber);
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
                string sqlQuery = @"UPDATE Employees SET FullName = @FullName, Gender = @Gender,
                       Email =  @Email, MobileNumber = @MobileNumber, DateOfJoining = @DateOfJoining, Salary = @Salary
                       WHERE Id = @Id ";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Id", employee.Id);
                sqlCommand.Parameters.AddWithValue("@FullName", employee.FullName);
                sqlCommand.Parameters.AddWithValue("@Gender", employee.Gender);
                sqlCommand.Parameters.AddWithValue("@Email", employee.Email);
                sqlCommand.Parameters.AddWithValue("@MobileNumber", employee.MobileNumber);
                sqlCommand.Parameters.AddWithValue("@DateOfJoining", employee.DateOfJoining);
                sqlCommand.Parameters.AddWithValue("@Salary", employee.Salary);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }
    }
}
