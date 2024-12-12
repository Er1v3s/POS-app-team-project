using System.Collections.ObjectModel;
using DataAccess.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using POS.Services.ToDoList;

namespace POS.ViewModels.ToDoList
{
    public class ToDoListViewModel : ViewModelBase
    {
        private readonly TaskManagerService _taskManager;

        private string newTaskContent;
        private ObservableCollection<ToDoListTask> todoListTaskCollection;

        public string NewTaskContent
        {
            get => newTaskContent;
            set => SetField(ref newTaskContent, value);
        }

        public ObservableCollection<ToDoListTask> TodoListTaskCollection
        {
            get => todoListTaskCollection;
            set => SetField(ref todoListTaskCollection, value);
        }

        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        public ToDoListViewModel(TaskManagerService taskManager)
        {
            _taskManager = taskManager;

            TodoListTaskCollection = [];
            AddTaskCommand = new RelayCommand(async _ => await AddTaskAsync());
            DeleteTaskCommand = new RelayCommand<ToDoListTask>(async (task) => await DeleteTaskAsync(task));

            _ = LoadTasksAsync();
        }

        private async Task LoadTasksAsync()
        {
            TodoListTaskCollection.Clear();
            var tasks = await _taskManager.GetTaskList();

            foreach (var task in tasks)
            {
                TodoListTaskCollection.Add(task);
            }
        }

        private async Task AddTaskAsync()
        {
            await _taskManager.CreateTask(NewTaskContent);
            NewTaskContent = string.Empty;
            await LoadTasksAsync();
        }

        private async Task DeleteTaskAsync(ToDoListTask task)
        {
            TodoListTaskCollection.Remove(task);
            await _taskManager.DeleteTask(task);
        }
    }
}
