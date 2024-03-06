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
       // this line added to remove filter 
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
