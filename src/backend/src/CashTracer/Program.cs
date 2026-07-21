using CashTracer.Api;
using CashTracer.Application;
using CashTracer.Infrastructure;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddApi();
var app = builder.Build();
app.ConfigureApi();
await app.RunAsync();