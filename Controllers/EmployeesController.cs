using EmployeeManagementSystem.Interface;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Repositories;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EmployeeManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;
        private readonly IAwsS3Service _awsS3Service;

        public EmployeesController(IEmployeeRepository repository, IAwsS3Service awsS3Service)
        {
            _repository = repository;
           
            _awsS3Service = awsS3Service;
        }

        // POST: api/Employees/upload-photo/5
        [HttpPost("upload-photo/{id}")]
        public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is missing");

            var employee = await _repository.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound($"Employee with ID {id} not found");

            var fileName = $"employee-{id}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fileUrl = await _awsS3Service.UploadFileAsync(file, fileName);

            if (fileUrl == null)
                return StatusCode(500, "Error uploading file to S3");

            employee.PhotoUrl = fileUrl;
            await _repository.UpdateEmployeeAsync(employee);

            Log.Information("Photo uploaded and linked to employee ID: {Id}", id);

            return Ok(new { message = "Photo uploaded successfully", photoUrl = fileUrl });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _repository.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _repository.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound("Employee not found");

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Employee employee)
        {
            var success = await _repository.AddEmployeeAsync(employee);
            if (!success)
                return BadRequest("Unable to add employee");

            return Ok("Employee added successfully");
        }

        [HttpPut]
        public async Task<IActionResult> Update(Employee employee)
        {
            var success = await _repository.UpdateEmployeeAsync(employee);
            if (!success)
                return NotFound("Employee not found");

            return Ok("Employee updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteEmployeeAsync(id);
            if (!success)
                return NotFound("Employee not found");

            return Ok("Employee deleted successfully");
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployee([FromQuery] string? name, [FromQuery] string? department)
        {
            var results = await _repository.SearchEmployeeAsync(name, department);
            return Ok(results);
        }

        [HttpGet("sorted-by-salary")]
        public async Task<IActionResult> GetSortedBySalary()
        {
            var results = await _repository.GetEmployeesSortedBySalaryDescAsync();
            return Ok(results);
        }

        [HttpGet("grouped-by-department")]
        public async Task<IActionResult> GetByDepartment()
        {
            var results = await _repository.GetEmployeesByDepartmentAsync();
            return Ok(results);
        }

        [HttpGet("top-salaries/{n}")]
        public async Task<IActionResult> GetTopSalaries(int n)
        {
            var results = await _repository.FindTopSalariesAsync(n);
            return Ok(results);
        }

        [HttpGet("average-salary-by-department")]
        public async Task<IActionResult> GetAverageSalary()
        {
            var results = await _repository.CalculateAverageSalaryByDepartmentAsync();
            return Ok(results);
        }

        [HttpGet("joined-before/{year}")]
        public async Task<IActionResult> GetJoinedBefore(int year)
        {
            var results = await _repository.GetEmployeesJoinedBeforeYearAsync(year);
            return Ok(results);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedEmployees([FromQuery] int skip = 0, [FromQuery] int take = 5)
        {
            var employees = await _repository.GetPagedEmployeesAsync(skip, take);
            return Ok(employees);
        }
    }
}
