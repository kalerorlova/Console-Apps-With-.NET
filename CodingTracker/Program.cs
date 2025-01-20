using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<AppController>();
builder.Services.AddSingleton<CodingSessionController>();
builder.Services.AddSingleton<ICodingSessionRepository, CodingSessionRepository>();
using var host = builder.Build();
var app = host.Services.GetService<AppController>()!;
app.Run();

