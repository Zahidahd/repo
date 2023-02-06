using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly string _connectionString;

        public DoctorRepository(string connectionString)
        {
            _connectionString = connectionString;

        }

        public DataTable GetAllDoctors()
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Doctors", sqlConnection);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);
                DataTable copyDataTable = dataTable.Copy();
                sqlDataAdapter.Fill(copyDataTable);
                return copyDataTable;
            }
        }

        public int GetDoctorsCount()
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = "SELECT COUNT(*) FROM Doctors";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlConnection.Open();
                int doctorCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return doctorCount;
            }
        }

        public DataTable GetDoctorDetailById(int doctorId)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Doctors WHERE Id = @doctorId", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@doctorId", doctorId);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataTable GetDoctorsByDepartmentByDoctorName(string department, string doctorName)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new(@"SELECT * FROM Doctors WHERE Department = @department
                                                  AND Name = @doctorName ", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@department", department);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@doctorName", doctorName);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataTable GetDoctorsNameListByDepartment(string department)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Doctors WHERE Department = @department", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@department", department);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        public string GetDoctorFullNameByDoctorId(int doctorId)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = "SELECT Name FROM Doctors WHERE Id = @doctorId";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@doctorId", doctorId);
                sqlConnection.Open();
                string doctorFullName = Convert.ToString(sqlCommand.ExecuteScalar());
                string subStringDoctorFullName = doctorFullName.Substring(3);
                sqlConnection.Close();
                return subStringDoctorFullName;
            }
        }

        public int Add(DoctorDto doctor)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string completeFullName = doctor.FullName.Insert(0, "Mr.");
                string completeFullName2 = completeFullName.Replace("Mr.", "Janab.");

                string sqlQuery = @"INSERT INTO Doctors(Name, Email, Gender, Department, City)
                        VALUES(@FullName, @Email, @Gender, @Department, @City)
                        Select Scope_Identity()";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@FullName", completeFullName2);
                sqlCommand.Parameters.AddWithValue("@Email", doctor.Email);
                sqlCommand.Parameters.AddWithValue("@Gender", doctor.Gender);
                sqlCommand.Parameters.AddWithValue("@Department", doctor.Department);
                sqlCommand.Parameters.AddWithValue("@City", doctor.City);
                sqlConnection.Open();
                doctor.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return doctor.Id;
            }
        }

        public void Update(DoctorDto doctor)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = @"UPDATE Doctors SET Name = @FullName, Email = @Email,
                        Gender = @Gender, Department = @Department, City = @City
                        WHERE Id = @Id ";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Id", doctor.Id);
                sqlCommand.Parameters.AddWithValue("@FullName", doctor.FullName);
                sqlCommand.Parameters.AddWithValue("@Email", doctor.Email);
                sqlCommand.Parameters.AddWithValue("@Gender", doctor.Gender);
                sqlCommand.Parameters.AddWithValue("@Department", doctor.Department);
                sqlCommand.Parameters.AddWithValue("@City", doctor.City);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }
    }
}
