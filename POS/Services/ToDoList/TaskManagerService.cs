using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using POS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Services.ToDoList
{
    public class TaskManagerService
    {
        private readonly AppDbContext _dbContext;

        public MyObservableCollection<ToDoListTask> ToDoTaskCollection { get; }

        public TaskManagerService(AppDbContext dbContext)
        {
            _dbContext = dbContext;

            ToDoTaskCollection = new();
            _ = GetAllTasks();
        }

        public async Task GetAllTasks()
        {
            var tasks = await GetAllTasksFromDbAsync();

            ToDoTaskCollection.Clear();
            await ToDoTaskCollection.AddRangeWithDelay(tasks, 50);
        }

        public async Task CreateTaskAsync(string newTaskContent)
        {
            if (string.IsNullOrWhiteSpace(newTaskContent) || newTaskContent.Equals("Dodaj zadanie"))
                return;

            var newTask = new ToDoListTask
            {
                Content = newTaskContent,
                CreationDate = DateTime.Now,
                CompletionDate = null,
            };

            ToDoTaskCollection.Add(newTask);
            await _dbContext.ToDoListTasks.AddAsync(newTask);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(ToDoListTask task)
        {
            var taskFromDb = await _dbContext.ToDoListTasks.FindAsync(task.TodoTaskId);

            if (taskFromDb != null)
            {
                ToDoTaskCollection.Remove(task);
                taskFromDb.CompletionDate = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<List<ToDoListTask>> GetAllTasksFromDbAsync()
        {
            var tasks = await _dbContext.ToDoListTasks.Where(p => p.CompletionDate == null).ToListAsync();

            return tasks;
        }
    }
}