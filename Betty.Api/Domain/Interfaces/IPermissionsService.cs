using Betty.Api.Infrastructure.Data;

namespace Betty.Api.Domain.Interfaces
{
    public interface IPermissionsService
    {
        public Task<bool> HasPermissions(int userCode, int projectCode, PermisionType permission = PermisionType.Admin, bool throwException = true);
    }

    public enum PermisionType
    {
        Admin = 1
    }
}
