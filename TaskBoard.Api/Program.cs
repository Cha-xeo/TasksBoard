using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using TaskBoard.Api.Configuration;
using TaskBoard.Api.Data;
using TaskBoard.Api.Exceptions;
using TaskBoard.Api.Handler;
using TaskBoard.Api.Services;
using TaskBoard.Api.Services.Interface;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Custom services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITasksService, TasksService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme 
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Enter JWT Access Token",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});



// Auth
builder.Services.AddAuthorization();

// Bind configuration section into JwtOptions
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));
// Read JWT options for middleware setup
JwtOptions jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

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
    var keyFromEnv = Environment.GetEnvironmentVariable("Jwt__Key");
    if (string.IsNullOrEmpty(keyFromEnv))
        throw new InvalidOperationException("JWT Key environment variable not set for production.");
    jwtOptions.Key = keyFromEnv;

    builder.Services.AddDbContext<TasksContext>(options =>
        options.UseMySql(Environment.GetEnvironmentVariable("TaskDatabase") ?? throw new InvalidOperationException("Env veriable  'TaskDatabase' not found."), serverVersion) 
    );


    // Enforce HTTPS redirection
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
        options.HttpsPort = 443;
    });
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuers = jwtOptions.Issuer,
            ValidAudiences = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.Key)
            )
        };
        options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
    })
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

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
// Middlewares
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
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

app.MapGet("/", () => "Hello, World!");
app.MapGet("/secret", (ClaimsPrincipal user) => $"Hello {user.Identity?.Name}. My secret")
    .RequireAuthorization();
app.MapGet("/secret2", () => "This is a different secret!")
    .RequireAuthorization(p => p.RequireClaim("scope", "myapi:secrets"));

app.Run();