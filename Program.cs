using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TeleBrief.Commands;
using TeleBrief.Infrastructure;
using TeleBrief.Infrastructure.Data;
using TeleBrief.News;
using TeleBrief.Topics;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("app.settings.json", false, true)
    .AddJsonFile("local.settings.json", true, true)
    .Build();

var appConfig = new AppConfig();
configuration.Bind(appConfig);

services.AddSingleton(appConfig);
services.AddSingleton(KernelBuilder.BuildKernel(appConfig));
services.AddDbContext<AppDbContext>();
services.AddScoped<TopicAnalysisService>();
services.AddScoped<NewsService>();

using (var scope = services.BuildServiceProvider().CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(c =>
{
    c.AddCommand<NewsCommand>("news")
        .WithDescription("Get today's news summary from configured Telegram channels");
    c.AddCommand<HeartbeatCommand>("beat")
        .WithDescription("Get today's state of topics based on news summary");
});

return await app.RunAsync(args);