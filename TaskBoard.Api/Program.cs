using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TaskBoard.Api.Data;
using TaskBoard.Api.Exceptions;
using TaskBoard.Api.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Custom services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITasksService, TasksService>();

// Database connection
var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<TasksContext>(options =>
        options.UseMySql(builder.Configuration.GetConnectionString("TaskDatabase") ?? throw new InvalidOperationException("Connection string 'TaskDatabase' not found."), serverVersion)
    );
}
else
{
    builder.Services.AddDbContext<TasksContext>(options =>
        options.UseMySql(Environment.GetEnvironmentVariable("TaskDatabase") ?? throw new InvalidOperationException("Env veriable  'TaskDatabase' not found."), serverVersion) 
    );
}

// Controllers
builder.Services.AddControllers();
// Prevent self referencing loop
//builder.Services.AddControllers().AddNewtonsoftJson(delegate(MvcNewtonsoftJsonOptions options)
//{
//    options.SerializerSettings.ReferenceLoopHandling = (ReferenceLoopHandling)1;
//});
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Global error handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        // Get exception details
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error;

        // You can customize the status code based on exception type
        context.Response.StatusCode = exception switch
        {
            ApiException apiEx => apiEx.StatusCode,                 // Custom client errors
            DbUpdateException => 400,                               // database errors
            KeyNotFoundException => 404,                            // not found
            _ => 500                                                // general errors
        };

        // Log the exception (console, file, or use ILogger)
        Console.WriteLine($"Exception: {exception?.Message}");

        // Return consistent JSON
        await context.Response.WriteAsJsonAsync(new
        {
            error = exception?.Message,
            stackTrace = app.Environment.IsDevelopment() ? exception?.StackTrace : null
        });
    });
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Database handcheck
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<TasksContext>();
    try
    {
        if (context.Database.CanConnect())
        {
            Console.WriteLine("Database connection successful!");
            context.Database.EnsureCreated();
        }
        else
        {
            Console.WriteLine("Cannot connect to database.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Connection failed: {ex.Message}");
    }
}


app.Run();