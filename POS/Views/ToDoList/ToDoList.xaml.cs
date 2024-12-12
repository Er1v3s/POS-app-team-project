using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using POS.ViewModels.ToDoList;
using Microsoft.Extensions.DependencyInjection;
using POS.Helpers;

namespace POS.Views.ToDoList
{
    /// <summary>
    /// Logika interakcji dla klasy ToDoList.xaml
    /// </summary>
    public partial class ToDoList : UserControl
    {
        public ToDoList()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<ToDoListViewModel>();
        }

        private void AddTask_TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBoxHelper.SetPlaceholderOnLostFocus(sender, e);
        }

        private void AddTask_TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBoxHelper.SetPlaceholderOnFocus(sender, e);
        }

        private void AddTask_KeyUp(object sender, KeyEventArgs e)
        {
            var viewModel = (ToDoListViewModel)DataContext;

            if (e.Key == Key.Enter)
            {
                viewModel.AddTaskCommand.Execute(null);
            }
        }
    }
}
