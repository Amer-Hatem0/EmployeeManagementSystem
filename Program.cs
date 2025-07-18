using EmployeeManagementSystem.Interface;
using EmployeeManagementSystem.Repositories;
using EmployeeManagementSystem.Services;
using Serilog;

namespace EmployeeManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog from appsettings.json
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Host.UseSerilog();

            // ? Configure to listen on all interfaces on port 5000
            builder.WebHost.UseUrls("http://0.0.0.0:5000");

            // Register services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddSingleton<IAwsS3Service, AwsS3Service>();
            builder.Services.AddSingleton<JsonFileService>();

            var app = builder.Build();

            // Log environment info
            Log.Information("Environment: {Env}", app.Environment.EnvironmentName);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthorization();
            app.MapControllers();

            try
            {
                Log.Information("Application started");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
