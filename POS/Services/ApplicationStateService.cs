using System.Net.Http;
using System.Threading.Tasks;
using DataAccess;

namespace POS.Services
{
    public class ApplicationStateService
    {
        private readonly AppDbContext _dbContext;

        public bool IsInternetAvailable;
        public bool IsDatabaseAvailable;

        public ApplicationStateService(AppDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        public async Task GetInternetStatusAsync()
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync("http://www.google.com");

                if (response.IsSuccessStatusCode)
                    IsInternetAvailable = true;
            }
            catch
            {
                IsInternetAvailable = false;
            }
        }

        public void GetDatabaseStatusAsync()
        {
            try
            {
                if (_dbContext.Database.CanConnect())
                    IsDatabaseAvailable = true;
            }
            catch
            {
                IsDatabaseAvailable = false;
            }
        }
    }
}
