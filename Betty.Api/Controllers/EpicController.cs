using Betty.Api.Domain.Interfaces;
using Betty.Api.Domain.Services;
using Betty.Api.Infrastructure.Data;
using Betty.Api.Infrastructure.Exceptions;
using Betty.Api.Infrastructure.Utils;
using BettyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace Betty.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EpicController : Controller
    {
        private readonly ILogger<EpicController> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IDbGenericHandler _dbHandler;
        public EpicController(ILogger<EpicController> logger, IDbGenericHandler dbHandler, IPermissionsService permissionsService)
        {
            _logger = logger;
            _dbHandler = dbHandler;
            _permissionsService = permissionsService;
        }

        [HttpGet("GetEpicsByProject")]
        public async Task<IEnumerable<Epic>> GetEpicsByProject(int projectCode)
        {
            _ = await _permissionsService.HasPermissions(Utils.GetUserFromContext(User)?.UserId ?? 0, projectCode);
            return await _dbHandler.Query<Epic>(e => e.ProjectCode == projectCode);
        }

        [HttpGet("GetEpic")]
        public async Task<Epic> GetEpic(int epicId)
        {
            Epic epic = (await _dbHandler.Query<Epic>(p => p.EpicId == epicId)).FirstOrDefault() ?? throw new ItemNotFoundException("The epic doesn't exist.");
            _ = await _permissionsService.HasPermissions(Utils.GetUserFromContext(User)?.UserId ?? 0, epic.ProjectCode);
            return epic;
        }

        [HttpPost("CreateEpic")]
        public async Task<int> CreateEpic(Epic epic)
        {
            var _userFromContext = Utils.GetUserFromContext(User);
            _ = await _permissionsService.HasPermissions(_userFromContext.UserId, epic.ProjectCode);


            if (epic.Title is null || epic.Text is null)
                throw new ArgumentNullException("Fields cannot be null.");

            epic.CreatedDateTime = DateTime.Now;
            epic.CreatedByUser = _userFromContext.UserId;

            return await _dbHandler.Insert(epic);
        }

        [HttpPost("UpdateEpic")]
        public async Task<int> UpdateEpic(Epic epic)
        {
            var _userFromContext = Utils.GetUserFromContext(User);
            _ = await _permissionsService.HasPermissions(_userFromContext.UserId, epic.ProjectCode);

            if (epic.Title is null || epic.Text is null)
                throw new ArgumentNullException("Fields cannot be null.");

            var epicquery = (await _dbHandler.Query<Epic>(e => e.EpicId == epic.EpicId)).FirstOrDefault() ?? throw new UnexpectedDbException("Epic to update doesn't found.");

            epic.ModifiedDateTime = DateTime.Now;
            epic.ModifiedByUser = _userFromContext.UserId;
            epic.ProjectCode = epicquery.ProjectCode;

            return await _dbHandler.Update(epic, p => p.EpicId == epic.EpicId);
        }
    }
}
