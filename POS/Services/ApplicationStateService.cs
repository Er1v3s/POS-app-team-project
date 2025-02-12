using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DataAccess;

namespace POS.Services
{
    public class ApplicationStateService : INotifyPropertyChanged
    {
        private readonly AppDbContext _dbContext;

        private bool isInternetAvailable;
        private bool isDatabaseAvailable;

        public bool IsInternetAvailable
        {
            get => isInternetAvailable;
            set => SetField(ref isInternetAvailable, value);
        }

        public bool IsDatabaseAvailable
        {
            get => isDatabaseAvailable;
            set => SetField(ref isDatabaseAvailable, value);
        }

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

        public async Task GetDatabaseStatusAsync()
        {
            try
            {
                if (await _dbContext.Database.CanConnectAsync())
                    IsDatabaseAvailable = true;
            }
            catch
            {
                IsDatabaseAvailable = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
