using HabitTracker.API.Models;
using HabitTracker.Application.Features.Auth.Commands.Login;
using HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogs;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HabitTracker.API.Configurations
{
    public class ApiResponseSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!context.Type.IsGenericType || context.Type.GetGenericTypeDefinition() != typeof(ApiResponse<>))
                return;

            var innerType = context.Type.GetGenericArguments()[0];
            var generator = context.SchemaGenerator;
            var repo = context.SchemaRepository;
            var innerSchema = generator.GenerateSchema(innerType, repo);

            // Định nghĩa schema chính xác
            schema.Properties["status"] = new OpenApiSchema { Type = "integer", Format = "int32" };
            schema.Properties["data"] = innerSchema;
            schema.Properties["data"].Nullable = true;
            schema.Properties["error"] = new OpenApiSchema { Type = "string", Nullable = true };
            schema.Properties["timestamp"] = new OpenApiSchema { Type = "string", Format = "date-time" };

            // Chỉ áp dụng ví dụ thành công cho các kiểu cụ thể (không phải object) và khi schema chưa có ví dụ
            if (innerType != typeof(object) && schema.Example == null)
            {
                schema.Example = new OpenApiObject
                {
                    ["status"] = new OpenApiInteger(200),
                    ["data"] = GetExampleForType(innerType, 200) ?? innerSchema.Example ?? new OpenApiObject
                    {
                        ["exampleField"] = new OpenApiString($"Example {innerType.Name} data")
                    },
                    ["error"] = new OpenApiNull(),
                    ["timestamp"] = new OpenApiString(DateTime.UtcNow.ToString("O"))
                };
            }
        }

        public static IOpenApiAny GetExampleForType(Type type, int statusCode)
        {
            // Ví dụ cho các mã trạng thái lỗi
            if (type == typeof(object) && statusCode != 200 && statusCode != 201)
            {
                return GetErrorExample(statusCode);
            }

            // Ví dụ cho các mã trạng thái thành công
            if (type == typeof(HabitViewModel))
            {
                return new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(1),
                    ["title"] = new OpenApiString("Drink Water 💧"),
                    ["description"] = new OpenApiString("Uống 2 lít nước mỗi ngày"),
                    ["frequency"] = new OpenApiString("Daily")
                };
            }
            else if (type == typeof(List<HabitViewModel>))
            {
                return new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["id"] = new OpenApiInteger(1),
                        ["title"] = new OpenApiString("Drink Water 💧"),
                        ["description"] = new OpenApiString("Uống 2 lít nước mỗi ngày"),
                        ["frequency"] = new OpenApiString("Daily")
                    },
                    new OpenApiObject
                    {
                        ["id"] = new OpenApiInteger(2),
                        ["title"] = new OpenApiString("Exercise 🏋️"),
                        ["description"] = new OpenApiString("Tập thể dục 30 phút mỗi ngày"),
                        ["frequency"] = new OpenApiString("Daily")
                    }
                };
            }
            else if (type == typeof(HabitLogViewModel))
            {
                return new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(1),
                    ["habitId"] = new OpenApiInteger(1),
                    ["date"] = new OpenApiString(DateTime.UtcNow.ToString("O")),
                    ["isCompleted"] = new OpenApiBoolean(true)
                };
            }
            else if (type == typeof(UserViewModel))
            {
                return new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(1),
                    ["username"] = new OpenApiString("john_doe"),
                    ["email"] = new OpenApiString("john.doe@example.com")
                };
            }
            else if (type == typeof(List<UserViewModel>))
            {
                return new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["id"] = new OpenApiInteger(1),
                        ["username"] = new OpenApiString("john_doe"),
                        ["email"] = new OpenApiString("john.doe@example.com")
                    },
                    new OpenApiObject
                    {
                        ["id"] = new OpenApiInteger(2),
                        ["username"] = new OpenApiString("jane_smith"),
                        ["email"] = new OpenApiString("jane.smith@example.com")
                    }
                };
            }

            return null;
        }

        public static IOpenApiAny GetErrorExample(int statusCode)
        {
            string errorMessage = statusCode switch
            {
                400 => "Yêu cầu không hợp lệ.",
                401 => "Không được phép truy cập.",
                404 => "Không tìm thấy tài nguyên.",
                500 => "Lỗi server nội bộ.",
                _ => "Lỗi không xác định."
            };

            return new OpenApiObject
            {
                ["status"] = new OpenApiInteger(statusCode),
                ["data"] = new OpenApiNull(),
                ["error"] = new OpenApiString(errorMessage),
                ["timestamp"] = new OpenApiString(DateTime.UtcNow.ToString("O"))
            };
        }
    }
}
