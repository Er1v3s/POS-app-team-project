using POS.Converter;
using POS.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            LoadTasks();
        }

        private void AddTask_TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBoxHelper.SetPlaceholderOnLostFocus(sender, e);
        }

        private void AddTask_TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBoxHelper.SetPlaceholderOnFocus(sender, e);
        }

        private void AddTask_ButtonClick(object sender, RoutedEventArgs e)
        {
            if(addTaskTextBox.Text != null && addTaskTextBox.Text != "Dodaj zadanie")
            {
                AddTask(new ToDoListTask { Content = addTaskTextBox.Text, CreationDate = DateTime.Now, CompletionDate = null });
            }
            else
            {
                // można dodać messageBox z prośbą o wpisanie zadania
            }
        }

        private void AddTask_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                AddTask(new ToDoListTask { Content = addTaskTextBox.Text, CreationDate = DateTime.Now, CompletionDate = null });
            }
        }

        private void DeleteTask_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (todoListDataGrid.SelectedItem != null)
            {
                var selectedTask = (ToDoListTask)todoListDataGrid.SelectedItem;
                todoListTaskCollection.Remove(selectedTask);

                using (var dbContext = new AppDbContext())
                {
                    var taskFromDb = dbContext.ToDoListTasks.Find(selectedTask.TodoTask_Id);

                    if (taskFromDb != null)
                    {
                        taskFromDb.CompletionDate = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                }
            }
            else { return; }
        }

        private void AddTask(ToDoListTask task)
        {
            if (!string.IsNullOrEmpty(addTaskTextBox.Text))
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.ToDoListTasks.Add(task);
                    dbContext.SaveChanges();
                }
            }
            else { return; }
            LoadTasks();
            addTaskTextBox.Text = "";
        }

        private void LoadTasks()
        {
            todoListTaskCollection.Clear();
            using (var dbContext = new AppDbContext())
            {
                var tasks = dbContext.ToDoListTasks.Where(p => p.CompletionDate == null).ToList();

                foreach (var task in tasks)
                {
                    todoListTaskCollection.Add(task);
                }
            }
        }
    }
}
