using System;
using DataAccess.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using POS.Services.ToDoList;
using POS.Utilities;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.ToDoList
{
    public class ToDoListViewModel : ViewModelBase
    {
        private readonly TaskManagerService _taskManager;

        private string newTaskContent;

        public string NewTaskContent
        {
            get => newTaskContent;
            set => SetField(ref newTaskContent, value);
        }

        public MyObservableCollection<ToDoListTask> ToDoTaskObservableCollection
        {
            get => _taskManager.ToDoTaskCollection;
        }

        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        public ToDoListViewModel(TaskManagerService taskManager)
        {
            _taskManager = taskManager;

            AddTaskCommand = new RelayCommandAsync(AddTaskAsync);
            DeleteTaskCommand = new RelayCommandAsync<ToDoListTask>(DeleteTaskAsync);
        }

        private async Task AddTaskAsync()
        {
            await _taskManager.CreateTaskAsync(NewTaskContent);
            ResetForm();
        }

        private async Task DeleteTaskAsync(ToDoListTask task)
        {
            await _taskManager.DeleteTaskAsync(task);
        }

        private void ResetForm()
        {
            NewTaskContent = String.Empty;
        }
    }
}
