using Microsoft.AspNetCore.Mvc;
using Task.Domain.Dto;
using Task.Domain.Models;

namespace Task.Application.Services
{
    public interface ITaskService
    {
        Task<MyTask> CreateTaskService(CreateTaskDto request, int userId);
        Task<List<MyTask>> GetAllTaskService(int userId);
        Task<MyTask?> GetTaskByIdService(int Id, int userId);
        Task<MyTask> UpdateTaskService(UpdateTaskDto request, int Id, int userId);
        Task<bool?> DeleteTaskService(int Id, int userId);
    }
}
