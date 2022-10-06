using System.Net.Mime;
using System.Text.Json;
using Catalog.Repositories;
using Catalog.Settings;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDbSettings"));

var settings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDBSettings>();
builder.Services.AddSingleton<IMongoClient>(ServiceProvider =>{
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddHealthChecks()
.AddMongoDb(settings.ConnectionString,
            name:"mongodb",
            timeout: TimeSpan.FromSeconds(3),
            tags: new[]{"ready"});

builder.Services.AddSingleton<IItemsRepository, MongoDbItemsRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/healthz/ready",new HealthCheckOptions{
    Predicate = (check) => check.Tags.Contains("ready"),
    ResponseWriter = async(context,report) =>
    {
        var result = JsonSerializer.Serialize(
            new {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    exception = entry.Value.Exception.Message,
                    duration = entry.Value.Duration.ToString()
                })
            }
        );
        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(result);
    }
});

app.MapHealthChecks("/healthz/live",new HealthCheckOptions{
    Predicate = (_) => false
});

app.MapControllers();

app.Run();
