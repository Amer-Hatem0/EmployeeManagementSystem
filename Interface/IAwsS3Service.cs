namespace EmployeeManagementSystem.Interface
{
    public interface IAwsS3Service
    {
        Task<string?> UploadFileAsync(IFormFile file, string fileName);
    }
}
