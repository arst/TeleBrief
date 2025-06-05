using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TeleBrief;
using TeleBrief.Commands;
using TeleBrief.Infrastructure;

var services = new ServiceCollection();

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("app.settings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .Build();

var appConfig = new AppConfig();
config.Bind(appConfig);

services.AddSingleton(appConfig);
services.AddSingleton(KernelBuilder.BuildKernel(appConfig));

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.AddCommand<NewsCommand>("news")
        .WithDescription("Get today's news summary from configured Telegram channels");
});

return await app.RunAsync(args);
