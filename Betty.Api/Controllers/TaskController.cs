﻿using Betty.Api.Domain.Interfaces;
using Betty.Api.Domain.Services;
using Betty.Api.Infrastructure.Data;
using Betty.Api.Infrastructure.Exceptions;
using Betty.Api.Infrastructure.Utils;
using BettyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Betty.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TaskController : Controller
    {
        private readonly ILogger<TaskController> _logger;
        private readonly IDbGenericHandler _dbHandler;
        private readonly IPermissionsService _permissionsService;
        private readonly ITaskService _taskService;
        public TaskController(ILogger<TaskController> logger, IDbGenericHandler dbHandler, IPermissionsService permissionsService, ITaskService taskService)
        {
            _logger = logger;
            _dbHandler = dbHandler;
            _permissionsService = permissionsService;
            _taskService = taskService;
        }

        [HttpGet("GetTasksByUS")]
        public async Task<IEnumerable<BettyApi.Models.Task>> GetTasksByUS(int userStory)
        {
            var queryTasks = await _dbHandler.Query<BettyApi.Models.Task>(e => e.ParentUserStoryCode == userStory);

            if (!queryTasks.Any()) throw new ItemNotFoundException("UserStory doestn't found or haven't permissions.");

            _ = await _permissionsService.HasPermissions(Utils.GetUserFromContext(User)?.UserId ?? 0, queryTasks.FirstOrDefault()?.ProjectCode ?? -1);

            return queryTasks;
        }

        [HttpGet("GetTasksByUser")]
        public async Task<IEnumerable<BettyApi.Models.Task>> GetTasksByUser(int userId)
        {
            var queryTasks = await _dbHandler.Query<BettyApi.Models.Task>(e => e.ResponsibleUserCode == userId);

            if (!queryTasks.Any()) throw new ItemNotFoundException("Tasks doestn't found or haven't permissions.");

            _ = await _permissionsService.HasPermissions(Utils.GetUserFromContext(User)?.UserId ?? 0, queryTasks.FirstOrDefault()?.ProjectCode ?? -1);

            return queryTasks;
        }

        [HttpGet("GetTask")]
        public async Task<BettyApi.Models.Task> GetTask(int taskId)
        {
            BettyApi.Models.Task task = (await _dbHandler.Query<BettyApi.Models.Task>(p => p.TaskId == taskId)).FirstOrDefault() ?? throw new ItemNotFoundException("The Task doesn't exist.");

            _ = await _permissionsService.HasPermissions(Utils.GetUserFromContext(User)?.UserId ?? 0, task.ProjectCode);
            return task;
        }

        [HttpPost("CreateTask")]
        public async Task<int> CreateTask(BettyApi.Models.Task task)
        {
            var _userFromContext = Utils.GetUserFromContext(User);

            if (task.Title is null || task.Text is null)
                throw new NullReferenceException("Fields cannot be null.");

            _ = await _permissionsService.HasPermissions(_userFromContext.UserId, task.ProjectCode);

            task.CreatedDateTime = DateTime.Now;
            task.CreatedByUser = _userFromContext.UserId;

            return await _dbHandler.Insert(task);
        }

        [HttpPost("UpdateTask")]
        public async Task<int> UpdateTask(BettyApi.Models.Task Task)
        {
            var _userFromContext = Utils.GetUserFromContext(User);

            if (Task.Title is null || Task.Text is null)
                throw new NullReferenceException("Fields cannot be null.");

            var TaskQuery = (await _dbHandler.Query<BettyApi.Models.Task>(e => e.TaskId == Task.TaskId)).FirstOrDefault() ?? throw new ItemNotFoundException("Task to update doesn't found.");

            _ = await _permissionsService.HasPermissions(_userFromContext.UserId, TaskQuery.ProjectCode);

            Task.ModifiedDateTime = DateTime.Now;
            Task.ModifiedByUser = _userFromContext.UserId;
            Task.ProjectCode = TaskQuery.ProjectCode;
            Task.ParentUserStoryCode = TaskQuery.ParentUserStoryCode;

            return await _dbHandler.Update(Task, p => p.TaskId == Task.TaskId);
        }

        [HttpGet("RemoveTask")]
        public async Task<int> RemoveTask(int taskId) => await _taskService.RemoveTask(t => t.TaskId == taskId, User);
        
    }
}
