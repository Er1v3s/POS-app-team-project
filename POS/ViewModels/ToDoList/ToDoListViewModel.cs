using System;
using System.Collections.ObjectModel;
using DataAccess.Models;
using DataAccess;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace POS.ViewModels.ToDoList
{
    public class ToDoListViewModel : ViewModelBase
    {
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

        public ToDoListViewModel()
        {
            TodoListTaskCollection = new ObservableCollection<ToDoListTask>();
            AddTaskCommand = new RelayCommand(async _ => await AddTaskAsync());
            DeleteTaskCommand = new RelayCommand<ToDoListTask>(async (task) => await DeleteTaskAsync(task));

            _ = LoadTasksAsync();
        }

        private async Task LoadTasksAsync()
        {
            TodoListTaskCollection.Clear();

            await using var dbContext = new AppDbContext();

            var tasks = dbContext.ToDoListTasks.Where(p => p.CompletionDate == null).ToList();

            foreach (var task in tasks)
            {
                TodoListTaskCollection.Add(task);
            }
        }

        private async Task AddTaskAsync()
        {
            if(string.IsNullOrWhiteSpace(NewTaskContent) || NewTaskContent.Equals("Dodaj zadanie"))
                return;

            var newTask = new ToDoListTask
            {
                Content = NewTaskContent,
                CreationDate = DateTime.Now,
                CompletionDate = null,
            };

            await using var dbContext = new AppDbContext();
            await dbContext.ToDoListTasks.AddAsync(newTask);
            await dbContext.SaveChangesAsync();
            
            NewTaskContent = string.Empty;
            await LoadTasksAsync();
        }

        private async Task DeleteTaskAsync(ToDoListTask task)
        {
            TodoListTaskCollection.Remove(task);

            await using var dbContext = new AppDbContext();

            var taskFromDb = await dbContext.ToDoListTasks.FindAsync(task.TodoTaskId);
            if (taskFromDb != null)
            {
                taskFromDb.CompletionDate = DateTime.Now;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
