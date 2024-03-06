using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FNAPPTraceLogs2
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            //  _logger.LogInformation("C# HTTP trigger function processed a request.");
            _logger.LogInformation("LogInformation");//
            _logger.LogWarning("LogWarning"); //
            _logger.LogCritical("LogCritical"); // 
            _logger.LogError("LogError");//
            _logger.LogDebug("***LogDebug****");
            _logger.LogTrace("====LogTrace===");

            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
