using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis;
using Separator = LiveCharts.Wpf.Separator;

namespace POS.Views.ReportsAndAnalysisPanel
{
    /// <summary>
    /// Logika interakcji dla klasy ReportsAndAnalysis.xaml
    /// </summary>
    public partial class ReportsAndAnalysis : Page
    {

        public ReportsAndAnalysis()
        {
            InitializeComponent();
            DataContext = new ReportsAndAnalysisViewModel();
        }
    }
}