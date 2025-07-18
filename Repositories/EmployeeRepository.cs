using EmployeeManagementSystem.Interface;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Serilog;

namespace EmployeeManagementSystem.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> _employees;
        private readonly JsonFileService _jsonFileService;

        public EmployeeRepository(JsonFileService jsonFileService)
        {
            _jsonFileService = jsonFileService;
            _employees = _jsonFileService.LoadData();
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            Log.Information("Fetching all employees");
            return await Task.Run(() => _employees.ToList());
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            Log.Information("Fetching employee with ID: {Id}", id);
            return await Task.Run(() =>
            {
                var emp = _employees.FirstOrDefault(e => e.Id == id);
                if (emp == null)
                    Log.Warning("Employee not found with ID: {Id}", id);
                return emp;
            });
        }

        public async Task<bool> AddEmployeeAsync(Employee employee)
        {
            return await Task.Run(() =>
            {
                try
                {
                    employee.Id = _employees.Any() ? _employees.Max(e => e.Id) + 1 : 1;
                    _employees.Add(employee);
                    _jsonFileService.SaveData(_employees);
                    Log.Information("Employee added: {@Employee}", employee);
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error adding employee: {@Employee}", employee);
                    return false;
                }
            });
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            return await Task.Run(() =>
            {
                var existing = _employees.FirstOrDefault(e => e.Id == employee.Id);
                if (existing == null)
                {
                    Log.Warning("Attempted to update non-existing employee with ID: {Id}", employee.Id);
                    return false;
                }

                existing.Name = employee.Name;
                existing.Department = employee.Department;
                existing.Salary = employee.Salary;
                existing.JoiningDate = employee.JoiningDate;

                _jsonFileService.SaveData(_employees);
                Log.Information("Employee updated: {@Employee}", employee);
                return true;
            });
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            return await Task.Run(() =>
            {
                var emp = _employees.FirstOrDefault(e => e.Id == id);
                if (emp == null)
                {
                    Log.Warning("Attempted to delete non-existing employee with ID: {Id}", id);
                    return false;
                }

                _employees.Remove(emp);
                _jsonFileService.SaveData(_employees);
                Log.Information("Employee deleted with ID: {Id}", id);
                return true;
            });
        }

        public async Task<IEnumerable<Employee>> SearchEmployeeAsync(string? name, string? department)
        {
            Log.Information("Searching employees by name: {Name}, department: {Department}", name, department);
            return await Task.Run(() =>
                _employees.Where(e =>
                    (string.IsNullOrEmpty(name) || e.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(department) || e.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
                ).ToList()
            );
        }

        public async Task<IEnumerable<Employee>> GetEmployeesSortedBySalaryDescAsync()
        {
            Log.Information("Fetching employees sorted by salary descending");
            return await Task.Run(() => _employees.OrderByDescending(e => e.Salary).ToList());
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync()
        {
            Log.Information("Fetching employees grouped by department");
            return await Task.Run(() => _employees.OrderBy(e => e.Department).ToList());
        }

        public async Task<IEnumerable<Employee>> FindTopSalariesAsync(int n)
        {
            Log.Information("Fetching top {Count} salaries", n);
            return await Task.Run(() => _employees.OrderByDescending(e => e.Salary).Take(n).ToList());
        }

        public async Task<IEnumerable<DepartmentSalaryAverage>> CalculateAverageSalaryByDepartmentAsync()
        {
            Log.Information("Calculating average salary by department");
            return await Task.Run(() =>
                _employees
                    .GroupBy(e => e.Department)
                    .Select(g => new DepartmentSalaryAverage
                    {
                        Department = g.Key,
                        AverageSalary = g.Average(e => e.Salary)
                    })
            );
        }

        public async Task<IEnumerable<Employee>> GetEmployeesJoinedBeforeYearAsync(int year)
        {
            Log.Information("Fetching employees who joined before year: {Year}", year);
            return await Task.Run(() =>
                _employees.Where(e => e.JoiningDate.Year < year).ToList()
            );
        }

        public async Task<IEnumerable<Employee>> GetPagedEmployeesAsync(int skip, int take)
        {
            Log.Information("Fetching paged employees: skip={Skip}, take={Take}", skip, take);
            return await Task.Run(() =>
                _employees.Skip(skip).Take(take).ToList()
            );
        }
    }
}
