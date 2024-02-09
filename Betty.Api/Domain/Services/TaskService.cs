using Betty.Api.Domain.Interfaces;
using Betty.Api.Infrastructure.Data;
using Betty.Api.Infrastructure.Utils;
using BettyApi.Models;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Betty.Api.Domain.Services
{
    public class TaskService : ITaskService
    {
        private readonly IDbGenericHandler _dbHandler;

        private readonly IPermissionsService _permissionsService;
        public TaskService(IDbGenericHandler dbHandler, IPermissionsService permissionsService)
        {
            _dbHandler = dbHandler;
            _permissionsService = permissionsService;
        }

        public async Task<int> RemoveTask(Expression<Func<BettyApi.Models.Task, bool>> where, ClaimsPrincipal userContext) => await Remove(where, Utils.GetUserFromContext(userContext));
        public async Task<int> RemoveTask(Expression<Func<BettyApi.Models.Task, bool>> where, User user) => await Remove(where, user);

        private async Task<int> Remove(Expression<Func<BettyApi.Models.Task, bool>> where, User _userFromContext)
        {
            _ = await _permissionsService.HasPermissions(_userFromContext.UserId, (await _dbHandler.Query(where)).FirstOrDefault()?.ProjectCode ?? -1);
            
            //  This is a delegate
            var TaskQuery = await _dbHandler.Query(where, true, false);
            // Remove tasks related references
            _ = await _dbHandler.Delete<TaskRelatedByTask>(tr => TaskQuery.Any(t => t.TaskId == tr.TaskRelatedByTaskId));

            return await _dbHandler.Delete(where);
        }
    }
}
