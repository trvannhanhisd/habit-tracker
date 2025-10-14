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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using HabitTracker.API.Configurations;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc.Versioning;

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
    var provider = builder.Services.BuildServiceProvider()
        .GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        c.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = "HabitTracker API",
            Version = description.ApiVersion.ToString(),
            Description = description.IsDeprecated
                ? $"Phiên bản {description.ApiVersion} - Đã bị deprecated, vui lòng sử dụng phiên bản mới hơn."
                : $"Phiên bản {description.ApiVersion} - API theo dõi thói quen."
        });
    }

    // Giữ nguyên các cấu hình khác
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

    c.SchemaFilter<ApiResponseSchemaFilter>();
    c.OperationFilter<ErrorResponseOperationFilter>();
    //c.ExampleFilters();
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0); 
    options.AssumeDefaultVersionWhenUnspecified = true; 
    options.ReportApiVersions = true;
    // Cấu hình hỗ trợ nhiều chiến lược Versioning (URL, query paraeter, header)
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(), // ví dụ: /api/v1/Habit
        new QueryStringApiVersionReader("api-version"), // ví dụ: /api/Habit?api-version=1.0
        new HeaderApiVersionReader("X-Api-Version") // ví dụ: X-Api-Version: 1.0
    );
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; 
    options.SubstituteApiVersionInUrl = true; 
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
        .WithCronSchedule("0 59 23 * * ?"));
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
var app = builder.Build();


var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();


// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();

app.UseExceptionMiddleware();
app.UseAuthenticationErrorMiddleware(); 

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
