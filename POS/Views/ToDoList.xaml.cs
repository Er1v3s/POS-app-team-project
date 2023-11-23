using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ToDoList.xaml
    /// </summary>
    public partial class ToDoList : UserControl
    {
        ObservableCollection<ToDoListTask> todoListTaskCollection;
        public ToDoList()
        {
            InitializeComponent();

            todoListTaskCollection = new ObservableCollection<ToDoListTask>();
            todoListDataGrid.ItemsSource = todoListTaskCollection;
        }

        private void addTaskTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(addTaskTextBox.Text))
            {
                addTaskTextBox.Text = "Dodaj zadanie";
            }
        }

        private void addTaskTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (addTaskTextBox.Text.Length > 0)
            {
                addTaskTextBox.Text = "";
            }
        }

        private void addTask_ButtonClick(object sender, RoutedEventArgs e)
        {
            addTask(new ToDoListTask { content = addTaskTextBox.Text });
        }

        private void addTask_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                addTask(new ToDoListTask { content = addTaskTextBox.Text });
            }
        }

        private void deleteTask_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (todoListDataGrid.SelectedItem != null)
            {
                var selectedItem = (ToDoListTask)todoListDataGrid.SelectedItem;
                todoListTaskCollection.Remove(selectedItem);
            }
            else { return; }
        }

        private void addTask(ToDoListTask task)
        {
            if (addTaskTextBox.Text != "" && addTaskTextBox.Text != null)
            {
                todoListTaskCollection.Add(task);
                addTaskTextBox.Text = "";
            }
            else { return; }
        }
    }
}
