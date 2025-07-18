using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Interface
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<bool> AddEmployeeAsync(Employee employee);
        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<IEnumerable<Employee>> SearchEmployeeAsync(string? name, string? department);
        Task<IEnumerable<Employee>> GetEmployeesSortedBySalaryDescAsync();
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync();
        Task<IEnumerable<Employee>> FindTopSalariesAsync(int n);
        Task<IEnumerable<DepartmentSalaryAverage>> CalculateAverageSalaryByDepartmentAsync();
        Task<IEnumerable<Employee>> GetEmployeesJoinedBeforeYearAsync(int year);
        Task<IEnumerable<Employee>> GetPagedEmployeesAsync(int skip, int take);

    }
}
