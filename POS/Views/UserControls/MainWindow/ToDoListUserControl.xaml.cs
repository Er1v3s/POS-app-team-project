using System.Windows.Controls;
using POS.ViewModels.ToDoList;
using Microsoft.Extensions.DependencyInjection;

namespace POS.Views.UserControls.MainWindow
{
    /// <summary>
    /// Logika interakcji dla klasy ToDoList.xaml
    /// </summary>
    public partial class ToDoListUserControl : UserControl
    {
        public ToDoListUserControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<ToDoListViewModel>();
        }
    }
}
