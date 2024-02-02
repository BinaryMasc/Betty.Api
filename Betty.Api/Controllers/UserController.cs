using Betty.Api.Infrastructure.Data;
using BettyApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Betty.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        

        private readonly ILogger<UserController> _logger;
        private readonly IDbGenericHandler _dbHandler;

        public UserController(ILogger<UserController> logger, IDbGenericHandler dbHandler)
        {
            _logger = logger;
            _dbHandler = dbHandler;
        }

        [HttpGet("GetUsers")]
        public Task<IEnumerable<User>> GetUsers()
        {
            return _dbHandler.Query<User>(u => u.UserStateCode == 1);
        }

        [HttpPost("CreateUser")]
        public async Task<int> CreateUser(User user)
        {
            var queryResult = await _dbHandler.Query<User>(u => u.Username == user.Username);

            if (queryResult.Count() > 0)
            {
                throw new Exception("This username is already exist.");
            }

            var queryResult2 = await _dbHandler.Query<User>(u => u.Email == user.Email);

            if (queryResult2.Count() > 0)
            {
                throw new Exception("This Email is already exist.");
            }

            return await _dbHandler.Insert(user);
        }

        [HttpGet("GetUserInfoById")]
        public async Task<object> GetUserInfoById(int id)
        {
            return (await _dbHandler.Query<User>(u => u.UserStateCode == 1 && u.UserId == id)).FirstOrDefault() ?? new object();

        }
    }
}