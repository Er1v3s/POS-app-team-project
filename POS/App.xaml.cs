using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore.Infrastructure;
using POS.Data;

namespace POS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AppDbContext DbContext { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DbContext = new AppDbContext();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DbContext.Dispose();

            base.OnExit(e);
        }
    }
}
