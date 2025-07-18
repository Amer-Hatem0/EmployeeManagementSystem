using EmployeeManagementSystem.Models;
using System.Text.Json;

namespace EmployeeManagementSystem.Services
{
    public class JsonFileService
    {
        private readonly string _filePath;

        public JsonFileService()
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "employees.json");

            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");  
        }

        public List<Employee> LoadData()
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Employee>>(json) ?? new List<Employee>();
        }

        public void SaveData(List<Employee> employees)
        {
            var json = JsonSerializer.Serialize(employees, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
