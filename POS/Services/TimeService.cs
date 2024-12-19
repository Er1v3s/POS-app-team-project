using System;
using System.Timers;

namespace POS.Services
{
    public class TimeService
    {
        private readonly Timer _timer;

        public TimeService()
        {
            _timer = new Timer(60000);
            _timer.Elapsed += (s, e) => OnTimeUpdated();
            _timer.Start();
            CurrentTime = DateTime.Now;
        }

        public DateTime CurrentTime { get; private set; }

        public event Action TimeUpdated;

        private void OnTimeUpdated()
        {
            CurrentTime = DateTime.Now;
            TimeUpdated?.Invoke();
        }
    }

}
