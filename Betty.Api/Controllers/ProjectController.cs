using Betty.Api.Domain.Interfaces;
using Betty.Api.Infrastructure.Data;
using Betty.Api.Infrastructure.Utils;
using BettyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;

namespace Betty.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IDbGenericHandler _dbHandler;
        private readonly IPermissionsService _permissionsService;
        public ProjectController(ILogger<ProjectController> logger, IDbGenericHandler dbHandler, IPermissionsService permissionsService)
        {
            _logger = logger;
            _dbHandler = dbHandler;
            _permissionsService = permissionsService;
        }

        [HttpGet("GetProjects")]
        public Task<SqlResultCollection<Project>> GetProjects(int projectState)
        {
            return _dbHandler.Query<Project>(p => p.ProjectStateCode == projectState);
        }

        [HttpGet("GetProject")]
        public async Task<Project> GetProject(int projectId)
        {
            //_ = await _permissionsService.HasPermissions(Utils.GetUserFromContext(User)?.UserId ?? 0, projectId);
            return (await _dbHandler.Query<Project>(p => p.ProjectId == projectId)).FirstOrDefault() ?? throw new Exception("The project doesn't exist.");
        }

        [HttpPost("CreateProject")]
        public async Task<int> CreateProject(Project project)
        {
            var _userFromContext = Utils.GetUserFromContext(User);

            if (project.Title is null || project.Text is null)
                throw new Exception("Fields cannot be null.");

            project.CreatedDateTime = DateTime.Now;
            project.CreatedByUser = _userFromContext.UserId;

            //  Insert project before for create after the permissions
            if (await _dbHandler.Insert(project) < 1) throw new Exception("Error: the project wasn't inserted.");
            Project projectCreated = (await _dbHandler.Query<Project>(p => p.CreatedByUser == _userFromContext.UserId)).OrderByDescending(p => p.ProjectId).FirstOrDefault() ?? throw new Exception("Project tried inserted but doesn't found");

            //  Now create permissions and return affected rows (1)
            return await _dbHandler.Insert(new UserPermissionsByProject
            {
                PermissionTypeCode = 1,
                ProjectCode = projectCreated.ProjectId,
                UserCode = _userFromContext.UserId
            });

        }

        [HttpPost("UpdateProject")]
        public async Task<int> UpdateProject(Project project)
        {
            var _userFromContext = Utils.GetUserFromContext(User);
            _ = await _permissionsService.HasPermissions(_userFromContext.UserId, project.ProjectId);
            if (project.Title is null || project.Text is null)
                throw new Exception("Fields cannot be null.");

            project.ModifiedDateTime = DateTime.Now;
            project.ModifiedByUser = _userFromContext.UserId;

            return await _dbHandler.Update(project, p => p.ProjectId == project.ProjectId);
        }

    }
}
