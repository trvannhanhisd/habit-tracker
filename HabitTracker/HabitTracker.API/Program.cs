using HabitTracker.API.Extensions;
using HabitTracker.API.Middlewares;
using HabitTracker.Application;
using HabitTracker.Infrastructure;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;
using System.Text.Json;
using Hangfire;
using Quartz;
using HabitTracker.Infrastructure.Quartz;

var builder = WebApplication.CreateBuilder(args);


// cấu hình Log
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() 
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) // ẩn log framework
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Error)
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

// Cấu hình Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HabitTracker API", Version = "v1" });

    // Thêm hỗ trợ Bearer Token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Cấu hình Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// Cấu hình Quartz
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("ScheduleMissedHabitsJob");
    q.AddJob<ScheduleMissedHabitsJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts.ForJob(jobKey)
        .WithIdentity("ScheduleMissedHabitsJob-trigger")
        .WithCronSchedule("0 31 10 * * ?"));
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();


// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

// test route
app.MapGet("/", () => "Habit Tracker Background Jobs Active");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionMiddleware();
app.UseAuthenticationErrorMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
