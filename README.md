# Overview 
with reference to our docs: [Managing log levels
](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=linux#managing-log-levels)


![2024-03-06_12h59_54](https://github.com/Aghyad85/DotNetisolatedmissinglogs/assets/54501053/096d1e72-2d89-4272-9a73-a9e9ecec2d66)

The rest of your application continues to work with ILogger and `ILogger<T>`. However,<mark> by default, the Application Insights SDK adds a logging filter that instructs the logger to capture only warnings and more severe logs. If you want to disable this behavior, remove the filter rule as part of the service configuration:</mark>

# Default behavior: 

when you create a function App with an Isolated process below you the default code : 

**Programe.cs**
```
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
   .ConfigureServices(services =>
   {
       services.AddApplicationInsightsTelemetryWorkerService();
       services.ConfigureFunctionsApplicationInsights();
   
   })

    .Build();

host.Run();
```

**Then I created a default Http trigger function and added the below code** 


```
     _logger.LogInformation("C# HTTP trigger function processed a request.");
     _logger.LogCritical("LogCritical"); // 
     _logger.LogError("LogError");//
     _logger.LogWarning("LogWarning"); //
     _logger.LogInformation("LogInformation");//
     _logger.LogDebug("LogDebug");
     _logger.LogTrace("LogTrace");
```

After publishing it to the portal the result will be as the following :

![2024-03-06_12h18_18](https://github.com/Aghyad85/DotNetisolatedmissinglogs/assets/54501053/e193829c-742c-42b3-9a7e-90c00335d322)


 <mark> **As you can see LogInformation, LogDebug, and LogTrace are missing ?** </mark>


# How to show LogInformation logs 
to show LogInformation logs you need to change Program.cs as the following then publish to the portal .


```
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
   .ConfigureServices(services =>
   {
       services.AddApplicationInsightsTelemetryWorkerService();
       services.ConfigureFunctionsApplicationInsights();
       // this line added to remove the filter 
       services.Configure<LoggerFilterOptions>(options =>
       {
           // The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs. Application Insights requires an explicit override.
           // Log levels can also be configured using appsettings.json. For more information, see https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs
           LoggerFilterRule toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName
               == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

           if (toRemove is not null)
           {
               options.Rules.Remove(toRemove);
           }
       });
   })

    .Build();

host.Run();
```

Here we go the result from Portal will be :

![2024-03-06_12h27_47](https://github.com/Aghyad85/DotNetisolatedmissinglogs/assets/54501053/a1497815-86e3-456f-bc5a-cd4515057122)


# How to show LogDebug & LogTrace logs 
To show LogInformation logs you need to change Program.cs as the following then publish to the portal .


```
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
   .ConfigureServices(services =>
   {
       services.AddApplicationInsightsTelemetryWorkerService();
       services.ConfigureFunctionsApplicationInsights();
       //This line added to remove the filter 
       services.Configure<LoggerFilterOptions>(options =>
       {
           // The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs. Application Insights requires an explicit override.
           // Log levels can also be configured using appsettings.json. For more information, see https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs
           LoggerFilterRule toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName
               == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

           if (toRemove is not null)
           {
               options.Rules.Remove(toRemove);
           }
       });
   })

      //get settings ***
      .ConfigureAppConfiguration((HostBuilderContext hostContext, IConfigurationBuilder builder) =>
      {
          // Add custom configuration sources
          builder.AddJsonFile("appsettings.json", optional: true);
      })
    //set logging for the worker ***
    .ConfigureLogging((hostingContext, logging) =>
    {
        // Make sure the configuration of the appsettings.json file is picked up.
        logging.AddConfiguration(hostingContext.Configuration.GetSection("WorkerLogging"));
    })
    .Build();

host.Run();
```

**appsettings.json sample** 


```
{
  "WorkerLogging": {
    "LogLevel": {
      "Default": "Trace"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Trace"
      }
    }
  }
}
```
 
**Make sure to change appsettings.json properties to** 

![2024-03-06_12h31_18](https://github.com/Aghyad85/DotNetisolatedmissinglogs/assets/54501053/59342953-2fdc-4963-884d-8159a16b552b)

Here we go the result from Portal will be :
 
![2024-03-06_12h40_53](https://github.com/Aghyad85/DotNetisolatedmissinglogs/assets/54501053/301c79b6-613f-42ad-9363-9b81c8a96599)



# References 
- [Managing log levels](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=linux#managing-log-levels)
- [Unable to get Trace logs from worker](https://github.com/Azure/azure-functions-dotnet-worker/issues/2235)
