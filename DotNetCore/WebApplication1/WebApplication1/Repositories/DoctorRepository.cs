using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication1.DTO.InputDTO;
using WebApplication1.Enums;

namespace WebApplication1.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly string _connectionString;

        public DoctorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<DoctorDto> GetAllDoctorsAsList()
        {
            List<DoctorDto> doctors = new();
                
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Doctors", sqlConnection);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DoctorDto doctorDto = new();
                    doctorDto.Id = (int)dataTable.Rows[i]["Id"];
                    doctorDto.FullName = (string)dataTable.Rows[i]["Name"];
                    doctorDto.Gender = (GenderTypes)dataTable.Rows[i]["Gender"];
                    doctorDto.Email = (string)dataTable.Rows[i]["Email"];
                    doctorDto.RegistrationNumber = (int)dataTable.Rows[i]["RegistrationNumber"];
                    doctorDto.Department = (string)dataTable.Rows[i]["Department"];
                    doctorDto.City = (string)dataTable.Rows[i]["City"];

                    doctors.Add(doctorDto);
                }
                return doctors;
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

        public DoctorDto GetDoctorDetailById(int doctorId)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Doctors WHERE Id = @doctorId", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@doctorId", doctorId);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    DoctorDto doctorDto = new();
                    doctorDto.Id = (int)dataTable.Rows[0]["Id"];
                    doctorDto.FullName = (string)dataTable.Rows[0]["Name"];
                    doctorDto.Gender = (GenderTypes)dataTable.Rows[0]["Gender"];
                    doctorDto.Email = (string)dataTable.Rows[0]["Email"];
                    doctorDto.RegistrationNumber = (int)dataTable.Rows[0]["RegistrationNumber"];
                    doctorDto.Department = (string)dataTable.Rows[0]["Department"];
                    doctorDto.City = (string)dataTable.Rows[0]["City"];
                    return doctorDto;
                }
                else
                    return null;
            }
        }

        public List<DoctorDto> GetDoctorsByDepartmentByDoctorName(string department, string doctorName)
        {
            List<DoctorDto> doctors = new();

            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new(@"SELECT * FROM Doctors WHERE Department = @department
                                                  AND Name = @doctorName ", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@department", department);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@doctorName", doctorName);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DoctorDto doctorDto = new();
                    doctorDto.Id = (int)dataTable.Rows[i]["Id"];
                    doctorDto.FullName = (string)dataTable.Rows[i]["Name"];
                    doctorDto.Gender = (GenderTypes)dataTable.Rows[i]["Gender"];
                    doctorDto.Email = (string)dataTable.Rows[i]["Email"];
                    doctorDto.RegistrationNumber = (int)dataTable.Rows[i]["RegistrationNumber"];
                    doctorDto.Department = (string)dataTable.Rows[i]["Department"];
                    doctorDto.City = (string)dataTable.Rows[i]["City"];

                    doctors.Add(doctorDto);
                }
                return doctors;
            }
        }

        public List<DoctorDto> GetDoctorsNameListByDepartment(string department)
        {
            List<DoctorDto> doctors = new();

            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Doctors WHERE Department = @department", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@department", department);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DoctorDto doctorDto = new();
                    doctorDto.Id = (int)dataTable.Rows[i]["Id"];
                    doctorDto.FullName = (string)dataTable.Rows[i]["Name"];
                    doctorDto.Gender = (GenderTypes)dataTable.Rows[i]["Gender"];
                    doctorDto.Email = (string)dataTable.Rows[i]["Email"];
                    doctorDto.RegistrationNumber = (int)dataTable.Rows[i]["RegistrationNumber"];
                    doctorDto.Department = (string)dataTable.Rows[i]["Department"];
                    doctorDto.City = (string)dataTable.Rows[i]["City"];

                    doctors.Add(doctorDto);
                }
                return doctors;
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

                string sqlQuery = @"INSERT INTO Doctors(RegistrationNumber, Name, Email, Gender, Department, City)
                        VALUES(@RegistrationNumber, @FullName, @Email, @Gender, @Department, @City)
                        Select Scope_Identity()";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@RegistrationNumber", doctor.RegistrationNumber);
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
                string sqlQuery = @"UPDATE Doctors SET RegistrationNumber = @RegistrationNumber, Name = @FullName, 
                         Email = @Email, Gender = @Gender, Department = @Department, City = @City
                         WHERE Id = @Id ";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Id", doctor.Id);
                sqlCommand.Parameters.AddWithValue("@RegistrationNumber", doctor.RegistrationNumber);
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
