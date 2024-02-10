using Betty.Api.Infrastructure.Data;
using Betty.Api.Infrastructure.Exceptions;
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

        private readonly string messageIncorrectModel = "Incorrect concept model name.";
        public DictionaryController(ILogger<DictionaryController> logger, IDbGenericHandler dbHandler)
        {
            _logger = logger;
            _dbHandler = dbHandler;
        }

        [HttpGet("GetConcepts")]
        public async Task<IEnumerable<DictionaryBase>> GetConcepts(string modelName)
        {

            if (!modelName.StartsWith("c_"))
                throw new InvalidRequestException(messageIncorrectModel);

            Type modelType = Type.GetType("BettyApi.Models." + modelName) ?? throw new InvalidRequestException(messageIncorrectModel);

            if(!modelType.GetInheritanceChain().Where(t => t == typeof(DictionaryBase)).Any())
                throw new InvalidRequestException(messageIncorrectModel);

            MethodInfo queryMethod = typeof(IDbGenericHandler).GetMethod(nameof(IDbGenericHandler.QueryDictionary))?.MakeGenericMethod(modelType) ?? throw new InvalidRequestException(messageIncorrectModel);

            var task = (Task)(queryMethod.Invoke(_dbHandler, null) ?? throw new InvalidRequestException(messageIncorrectModel));
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            return (IEnumerable<DictionaryBase>)(resultProperty?.GetValue(task) ?? throw new InvalidRequestException(messageIncorrectModel));
        }
    }
}
