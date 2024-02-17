using Betty.Api.Domain.Interfaces;
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
    public class UserStoryController : Controller
    {
        private readonly ILogger<UserStoryController> _logger;
        private readonly IDbGenericHandler _dbHandler;
        private readonly IPermissionsService _permissionsService;
        private readonly ITaskService _taskService;
        public UserStoryController(ILogger<UserStoryController> logger, IDbGenericHandler dbHandler, IPermissionsService permissionsService, ITaskService taskService)
        {
            _logger = logger;
            _dbHandler = dbHandler;
            _permissionsService = permissionsService;
            _taskService = taskService;
        }

        [HttpGet("GetUserStoriesByEpic")]
        public async Task<IEnumerable<UserStory>> GetUserStoriesByEpic(int epicCode)
        {
            var us = await _dbHandler.Query<UserStory>(e => e.EpicCode == epicCode);
            _ = await _permissionsService.HasPermissions(Utils.GetUserFromContext(User)?.UserId ?? 0, us.FirstOrDefault()?.ProjectCode ?? -1);

            return us;
        }

        [HttpGet("GetUserStory")]
        public async Task<UserStory> GetUserStory(int userStoryId)
        {
            var us = (await _dbHandler.Query<UserStory>(p => p.UserStoryId == userStoryId)).FirstOrDefault() ?? throw new ItemNotFoundException("The UserStory doesn't exist.");
            _ = await _permissionsService.HasPermissions(Utils.GetUserFromContext(User)?.UserId ?? 0, us.ProjectCode);
            return us;
        }

        [HttpPost("CreateUserStory")]
        public async Task<int> CreateUserStory(UserStory userStory)
        {
            User _userFromContext = Utils.GetUserFromContext(User);
            _ = await _permissionsService.HasPermissions(_userFromContext.UserId, userStory.ProjectCode);


            if (userStory.Title is null || userStory.Text is null)
                throw new ArgumentNullException("Fields cannot be null.");

            userStory.CreatedDateTime = DateTime.Now;
            userStory.CreatedByUser = _userFromContext.UserId;

            return await _dbHandler.Insert(userStory);
        }

        [HttpPost("UpdateUserStory")]
        public async Task<int> UpdateUserStory(UserStory userStory)
        {
            User _userFromContext = Utils.GetUserFromContext(User);

            if (userStory.Title is null || userStory.Text is null)
                throw new ArgumentNullException("Fields cannot be null.");

            UserStory userStoryQuery = (await _dbHandler.Query<UserStory>(e => e.UserStoryId == userStory.UserStoryId))?.FirstOrDefault() ?? throw new ItemNotFoundException("UserStory to update doesn't found.");

            _ = await _permissionsService.HasPermissions(_userFromContext.UserId, userStoryQuery?.ProjectCode ?? -1);

            userStory.ModifiedDateTime = DateTime.Now;
            userStory.ModifiedByUser = _userFromContext.UserId;
            userStory.ProjectCode = userStoryQuery?.ProjectCode ?? throw new ArgumentNullException("ProjectCode cannot be null.");
            userStory.EpicCode = userStoryQuery.EpicCode;

            return await _dbHandler.Update(userStory, p => p.UserStoryId == userStory.UserStoryId);
        }

        [HttpGet("RemoveUS")]
        public async Task<int> RemoveUS(int usId)
        {
            User _userFromContext = Utils.GetUserFromContext(User);

            var usQuery = (await _dbHandler.Query<UserStory>(e => e.UserStoryId == usId)).FirstOrDefault() ?? throw new ItemNotFoundException("US to remove doesn't found.");
            _ = await _permissionsService.HasPermissions(_userFromContext.UserId, usQuery.ProjectCode);

            //  remove related tasks
            //_ = await _taskService.RemoveTask();

            return await _dbHandler.Delete<UserStory>(p => p.UserStoryId == usId);
        }

        [HttpGet("AutocompleteUS")]
        public async Task<Dictionary<string, object>[]> AutocompleteUS(string UsName, int projectId)
        {
            _ = await _permissionsService.HasPermissions(Utils.GetUserFromContext(User).UserId, projectId);
            return await _dbHandler.Query<UserStory, object>(p => p.Title.StartsWith(UsName) && p.ProjectCode == projectId, p => new { p.UserStoryId, p.Title }, rows: 10);
        }

        [HttpGet("SearchUS")]
        public async Task<Dictionary<string, object>[]> SearchUS(string UsName, int projectId)
        {
            _ = await _permissionsService.HasPermissions(Utils.GetUserFromContext(User).UserId, projectId);
            return await _dbHandler.Query<UserStory, object>(p => p.Title.Contains(UsName) && p.ProjectCode == projectId, p => new { p.UserStoryId, p.Title }, rows: 10);
        }
    }
}
