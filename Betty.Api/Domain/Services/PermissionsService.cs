using Betty.Api.Domain.Interfaces;
using Betty.Api.Infrastructure.Data;
using BettyApi.Models;

namespace Betty.Api.Domain.Services
{
    public class PermissionsService : IPermissionsService
    {
        private IDbGenericHandler _dbHandler;
        public PermissionsService(IDbGenericHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public async Task<bool> HasPermissions(int userCode, int projectCode, PermisionType permission = PermisionType.Admin, bool throwException = true)
        {
            var projectPermissions = await _dbHandler.Query<UserPermissionsByProject>(p => p.UserCode == userCode && p.ProjectCode == projectCode);

            if (projectPermissions.Count() < 1)
            {
                return throwException ? throw new Exception("This user does not have permissions for this project.") : false;
            }
            else return true;
        }
    }
}
