using Betty.Api.Infrastructure.Data;
using BettyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json;
using Betty.Api.Infrastructure.Utils;
using Betty.Api.Infrastructure.Exceptions;

namespace Betty.Api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IDbGenericHandler _dbHandler;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<UserController> logger, IDbGenericHandler dbHandler, IConfiguration configuration)
        {
            _logger = logger;
            _dbHandler = dbHandler;
            _configuration = configuration;
        }

        [HttpGet("GetUsers")]
        public Task<SqlResultCollection<User>> GetUsers()
        {
            var usr = Utils.GetUserFromContext(User);
            return _dbHandler.Query<User>(u => u.UserStateCode == 1);
        }

        [HttpGet("GetUserInfoById")]
        public async Task<object> GetUserInfoById(int id)
        {
            return (await _dbHandler.Query<User>(u => u.UserStateCode == 1 && u.UserId == id)).FirstOrDefault() ?? new object();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<string> Login(UserCredential pUser)
        {

            var hashPsw = Utils.GetHash(pUser.Password);
            var usrquery = await _dbHandler.Query<UserCredential>(u => u.Username == pUser.Username && u.Password == hashPsw);

            if (usrquery.Count() < 1) throw new UnauthorizedAccessException("incorrect username or password.");

            var userCredential = usrquery.First();
            User user = (await _dbHandler.Query<User>(u => u.UserId == userCredential.UserCode)).FirstOrDefault() ?? throw new ItemNotFoundException("User doesn't found.");

            return GenerateToken(user);
            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public async Task<int> CreateUser(User user, string password)
        {
            var queryResult = await _dbHandler.Query<User>(u => u.Username == user.Username);

            if (queryResult.Count() > 0)
            {
                throw new ValidationException("This username is already exist.");
            }

            var queryResult2 = await _dbHandler.Query<User>(u => u.Email == user.Email);

            if (queryResult2.Count() > 0)
            {
                throw new ValidationException("This Email is already exist.");
            }

            var hashPsw = Utils.GetHash(password);
            

            if (await _dbHandler.Insert(user) != 1)
                throw new UnexpectedDbException("Error creating the user " + user.Username);

            var userCreated = await _dbHandler.Query<User>(u => u.Username == user.Username);



            var usrCredentials = new UserCredential
            {
                Password = hashPsw,
                Username = user.Username,
                UserCode = userCreated.FirstOrDefault()?.UserId ?? throw new UnexpectedDbException("User created hasn't returned data.")
            };

            var rowsAffected = await _dbHandler.Insert(usrCredentials);
            if (rowsAffected > 0) _logger.LogInformation($"User created {user.UserId} - {user.Username} - {user.Email}");

            return rowsAffected;
        }

        private string GenerateToken(User user)
        {

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Get<string>());
            var issuer = _configuration.GetSection("Jwt:Issuer").Get<string>();

            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(issuer,
              issuer,
              claims: new[] { new Claim(typeof(User).ToString(), JsonConvert.SerializeObject(user)) },
              signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            return token;
        }

    }
}