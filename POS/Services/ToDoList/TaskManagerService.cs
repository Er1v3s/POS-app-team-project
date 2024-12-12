using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Services.ToDoList
{
    public class TaskManagerService(AppDbContext dbContext)
    {
        public async Task<List<ToDoListTask>> GetTaskList()
        {
            List<ToDoListTask> todoTaskList = [];

            var tasks = await dbContext.ToDoListTasks.Where(p => p.CompletionDate == null).ToListAsync();

            todoTaskList.AddRange(tasks);

            return todoTaskList;
        }

        public async Task CreateTask(string newTaskContent)
        {
            if (string.IsNullOrWhiteSpace(newTaskContent) || newTaskContent.Equals("Dodaj zadanie"))
                return;

            var newTask = new ToDoListTask
            {
                Content = newTaskContent,
                CreationDate = DateTime.Now,
                CompletionDate = null,
            };

            await dbContext.ToDoListTasks.AddAsync(newTask);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteTask(ToDoListTask task)
        {
            var taskFromDb = await dbContext.ToDoListTasks.FindAsync(task.TodoTaskId);
            if (taskFromDb != null)
            {
                taskFromDb.CompletionDate = DateTime.Now;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
