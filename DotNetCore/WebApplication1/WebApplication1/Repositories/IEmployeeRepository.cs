using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public interface IEmployeeRepository
    {
        public DataTable GetAllEmployees();
        public int GetEmployeesCount();
        public DataTable GetEmployeeDetailById(int employeeId);
        public DataTable GetEmployeesDetailByGenderBySalary(string gender, int salary);
        public DataTable GetEmployeesBySalaryRange(int minimumSalary, int maximumSalary);
        public string GetEmployeeFullNameById(int employeeId);
        public int Add(EmployeeDto employee);
        public void Update(EmployeeDto employee);
    }
}
