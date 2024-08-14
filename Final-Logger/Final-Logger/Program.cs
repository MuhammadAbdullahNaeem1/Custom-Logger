
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IAppLogger>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
    var baseLogPath = configuration["ConnectionStrings:BaseLogPath"] ?? "";
    return new Logger(connectionString, baseLogPath);
});
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{    
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Final Logger API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Final Logger API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();

app.Run();
