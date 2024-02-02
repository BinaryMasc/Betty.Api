using Betty.Api.Infrastructure.Data;
using BettyApi.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Task = System.Threading.Tasks.Task;

namespace Betty.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DictionaryController : Controller
    {
        private readonly ILogger<DictionaryController> _logger;
        private readonly IDbGenericHandler _dbHandler;
        public DictionaryController(ILogger<DictionaryController> logger, IDbGenericHandler dbHandler)
        {
            _logger = logger;
            _dbHandler = dbHandler;
        }

        [HttpGet("GetConcepts")]
        public async Task<IEnumerable<DictionaryBase>> GetConcepts(string modelName)
        {
            if (!modelName.StartsWith("c_"))
                throw new Exception("Invalid name model");

            Type modelType = Type.GetType("BettyApi.Models." + modelName) ?? throw new Exception("Invalid name model");

            if(!modelType.GetInheritanceChain().Where(t => t == typeof(DictionaryBase)).Any())
                throw new Exception("Invalid name model");

            MethodInfo queryMethod = typeof(IDbGenericHandler).GetMethod(nameof(IDbGenericHandler.QueryDictionary))?.MakeGenericMethod(modelType) ?? throw new Exception("Invalid name model");

            var task = (Task)(queryMethod.Invoke(_dbHandler, null) ?? throw new Exception("Invalid name model"));
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            return (IEnumerable<DictionaryBase>)(resultProperty?.GetValue(task) ?? throw new Exception("Invalid name model"));
        }
    }
}
