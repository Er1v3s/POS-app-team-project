using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy AddEditEmployeeWindow.xaml
    /// </summary>
    public partial class AddEditEmployeeWindow : Window
    {
        public AddEditEmployeeWindow()
        {
            InitializeComponent();
        }

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddNewEmployee_ButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
