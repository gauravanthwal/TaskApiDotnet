using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Domain.Dto;
using Task.Domain.Models;
using Task.Infrastructure;

namespace Task.Application.Services
{
    public class TaskService : ITaskService
    {
        DatabaseContext _context;
        public TaskService(DatabaseContext context) { 
            _context = context;
        }
        
        public async Task<MyTask?> CreateTaskService(CreateTaskDto req, int userId)
        {
            MyTask newTask = new MyTask
            {
                Task = req.Task,
                IsCompleted = false,
                CreatedBy = userId
            };

            await _context.Tasks.AddAsync(newTask);

            await _context.SaveChangesAsync();


            return newTask;
        }

        public async Task<bool?> DeleteTaskService(int Id, int userId)
        {
            MyTask? task = await this.GetTaskByIdService(Id, userId);

            if (task is null) return false;

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<MyTask>> GetAllTaskService(int userId)
        {
            List<MyTask> tasks = await _context.Tasks.Where(t=> t.CreatedBy== userId).ToListAsync();

            return tasks;
        }

        public async Task<MyTask?> GetTaskByIdService(int Id, int userId)
        {
            MyTask? task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == Id && t.CreatedBy == userId);

            return task;
        }

        public async Task<MyTask?> UpdateTaskService(UpdateTaskDto request, int Id, int userId)
        {
            MyTask? task = await this.GetTaskByIdService(Id, userId);

            if (task is null) return null;

            task.Task = request.Task;
            task.IsCompleted = true;

            await _context.SaveChangesAsync();

            return task;
        }
    }
}
