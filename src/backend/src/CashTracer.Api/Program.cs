using CashTracer.Api.Extensions;
using CashTracer.Application;
using CashTracer.Infrastructure;
using CashTracer.Infrastructure.Persistence.Sqlite;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddSqlitePersistence();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddApi();
var app = builder.Build();
app.ConfigureApi();
await app.RunAsync();