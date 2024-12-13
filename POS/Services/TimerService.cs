using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace POS.Services
{
    public class TimerService
    {
        private readonly DispatcherTimer _timer;
        private readonly Action<string, string> _updateDateTimeCallback;

        public TimerService(Action<string, string> updateDateTimeCallback)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += async (sender, args) => await UpdateDateTimeAsync();
            _updateDateTimeCallback = updateDateTimeCallback;
        }

        public void Start() => _timer.Start();

        private async Task UpdateDateTimeAsync()
        {
            string date = DateTime.Now.ToString("dd.MM.yyyy");
            string time = DateTime.Now.ToString("HH:mm:ss");

            await Task.Run(() => _updateDateTimeCallback(date, time));
        }
    }
}
