using POS.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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

namespace POS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ObservableCollection<ToDoListTask> toDoListTasks = new ObservableCollection<ToDoListTask>();

            // Create todo list DataGrid Item Info

            toDoListTasks.Add(new ToDoListTask { content = "Zrobić zamówienie" });
            toDoListTasks.Add(new ToDoListTask { content = "Wezwać serwis do nalewaka na piwo" });
            toDoListTasks.Add(new ToDoListTask { content = "Wysłać faktury do księgowej" });
            toDoListTasks.Add(new ToDoListTask { content = "Sprawdzić terminy ważności w lodówce" });
            toDoListTasks.Add(new ToDoListTask { content = "Wynieść śmieci" });

            todoListDataGrid.ItemsSource = toDoListTasks;
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

        private void Move_To_Sales_Panel(object sender, RoutedEventArgs e)
        {
            LoginPanel loginPanel = new LoginPanel();
            loginPanel.Show();

            //Views.SalesPanel salesPanel = new Views.SalesPanel();
            //salesPanel.Show();

            //Window.GetWindow(this).Close();
        }

        private void Turn_Off_Application(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ChangeFrameSource(Uri newSource)
        {
            try
            {
                frame.Source = newSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}");
            }
        }

        private void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string uri)
            {
                try
                {
                    Uri newFrameSource = new Uri(uri, UriKind.RelativeOrAbsolute);
                    ChangeFrameSource(newFrameSource);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd: {ex.Message}");
                }
            }
        }
    }

    public class ToDoListTask
    {
        public string content { get; set; }
    }
}
