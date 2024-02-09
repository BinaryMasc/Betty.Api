using Betty.Api.Infrastructure.Utils;
using BettyApi.Models;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Betty.Api.Domain.Interfaces
{
    public interface ITaskService
    {
        public Task<int> RemoveTask(Expression<Func<BettyApi.Models.Task, bool>> where, ClaimsPrincipal userContext);
        public Task<int> RemoveTask(Expression<Func<BettyApi.Models.Task, bool>> where, User user);
    }
}
