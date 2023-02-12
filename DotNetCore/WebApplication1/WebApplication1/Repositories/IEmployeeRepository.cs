using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public interface IEmployeeRepository
    {
        public List<EmployeeDto> GetAllEmployeesAsList();
        public EmployeeDto GetEmployeeDetailById(int id);
        public int GetEmployeesCount();
        public List<EmployeeDto> GetEmployeesDetailByGenderBySalary(int gender, int salary);
        public List<EmployeeDto> GetEmployeesBySalaryRange(int minimumSalary, int maximumSalary);
        public string GetEmployeeFullNameById(int employeeId);
        public int Add(EmployeeDto employee);
        public void Update(EmployeeDto employee);
    }
}
