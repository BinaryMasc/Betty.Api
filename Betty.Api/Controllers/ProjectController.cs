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

        public ProjectController(ILogger<ProjectController> logger, IDbGenericHandler dbHandler)
        {
            _logger = logger;
            _dbHandler = dbHandler;
        }

        [HttpGet("GetProjects")]
        public Task<IEnumerable<Project>> GetProjects(int projectState)
        {
            return _dbHandler.Query<Project>(p => p.ProjectStateCode == projectState);
        }

        [HttpGet("GetProject")]
        public async Task<Project> GetProject(int projectId)
        {
            return (await _dbHandler.Query<Project>(p => p.ProjectId == projectId)).FirstOrDefault() ?? throw new Exception("The project doesn't exist.");
        }

        [HttpPost("CreateProject")]
        public Task<int> CreateProject(Project project)
        {
            var _userFromContext = Utils.GetUserFromContext(User) ?? throw new Exception("Invalid token or not deserializable.");

            var usr = Utils.GetUserFromContext(User);
            if (project.Title is null || project.Text is null)
                throw new Exception("Fields cannot be null.");

            project.CreatedDateTime = DateTime.Now;
            project.CreatedByUser = _userFromContext.UserId;

            return _dbHandler.Insert(project);
        }

        [HttpPost("UpdateProject")]
        public Task<int> UpdateProject(Project project)
        {
            var _userFromContext = Utils.GetUserFromContext(User) ?? throw new Exception("Invalid token or not deserializable.");

            if (project.Title is null || project.Text is null)
                throw new Exception("Fields cannot be null.");

            project.ModifiedDateTime = DateTime.Now;
            project.ModifiedByUser = _userFromContext.UserId;

            return _dbHandler.Update(project, p => p.ProjectId == project.ProjectId);
        }

    }
}
