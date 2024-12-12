using Microsoft.Extensions.Configuration;

namespace DataAccess
{
    public class DatabaseConfiguration
    {
        public string GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            string environment = configuration["Environment"];
            
            return configuration[$"ConnectionStrings:{environment}"];
        }
    }
}
