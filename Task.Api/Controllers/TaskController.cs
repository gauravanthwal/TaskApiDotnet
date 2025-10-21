using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task.Application.Services;
using Task.Domain.Dto;
using Task.Domain.Models;

namespace Task.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        readonly ITaskService taskService;
        public TaskController(ITaskService _taskService)
        {
            taskService = _taskService;
        }


        // CREATE NEW TASK
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateTask(CreateTaskDto req)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            MyTask newTask = await taskService.CreateTaskService(req, int.Parse(userIdClaim!));

            if (newTask == null) return BadRequest("Unable to create Task");

            return Ok("task added successfully");
        }

        // GET ALL TASKS
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetMyTasks()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            List<MyTask> tasks = await taskService.GetAllTaskService(int.Parse(userIdClaim!));

            return Ok(tasks);
        }


        // GET TASK BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTask(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            MyTask? task = await taskService.GetTaskByIdService(id, int.Parse(userIdClaim!));

            if (task == null)
            {
                return BadRequest("No items found!");
            }

            return Ok(task);
        }


        // UPDATE TASK BY ID
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTask(UpdateTaskDto req, int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            MyTask task = await taskService.UpdateTaskService(req, id, int.Parse(userIdClaim!));

            if (task == null) return BadRequest("Couldn't update task");

            return Ok("Task updated successfully!");
        }


        //// DELETE TASK BY ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool? isDeleted = await taskService.DeleteTaskService(id, int.Parse(userIdClaim!));

            if (isDeleted == false) return BadRequest("Couldn't delete task");


            return Ok("task deleted successfully!");
        }


        // ADMIN ROLE ENDPOINT
        [HttpGet("get-admin")]
        [Authorize(Roles = "Admin")]
        public string GetAdmin()
        {
            return "This is only for Admin";
        }
    }
}