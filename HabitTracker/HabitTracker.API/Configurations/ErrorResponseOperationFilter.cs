using HabitTracker.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace HabitTracker.API.Configurations
{
    public class ErrorResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Lấy các thuộc tính ProducesResponseType từ method
            var producesResponseAttributes = context.MethodInfo
                .GetCustomAttributes<ProducesResponseTypeAttribute>()
                .Where(attr => attr.StatusCode >= 400) // Lấy các mã lỗi (400, 401, 404, v.v.)
                .ToList();

            foreach (var attr in producesResponseAttributes)
            {
                var statusCode = attr.StatusCode.ToString();
                var errorExample = ApiResponseSchemaFilter.GetErrorExample(attr.StatusCode);

                if (operation.Responses.TryGetValue(statusCode, out var response))
                {
                    // Thêm hoặc cập nhật ví dụ lỗi vào Content của response
                    if (!response.Content.TryGetValue("application/json", out var mediaType))
                    {
                        mediaType = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(ApiResponse<object>), context.SchemaRepository)
                        };
                        response.Content.Add("application/json", mediaType);
                    }

                    // Định nghĩa schema chính xác
                    mediaType.Schema.Properties["status"] = new OpenApiSchema { Type = "integer", Format = "int32" };
                    mediaType.Schema.Properties["data"] = new OpenApiSchema { Type = "object", Nullable = true };
                    mediaType.Schema.Properties["error"] = new OpenApiSchema { Type = "string", Nullable = true };
                    mediaType.Schema.Properties["timestamp"] = new OpenApiSchema { Type = "string", Format = "date-time" };

                    // Gán ví dụ lỗi
                    mediaType.Examples = new Dictionary<string, OpenApiExample>
                    {
                        {
                            "ErrorExample",
                            new OpenApiExample
                            {
                                Value = errorExample,
                                Summary = GetDescriptionForStatusCode(attr.StatusCode),
                                Description = "Ví dụ phản hồi lỗi"
                            }
                        }
                    };

                    // Cập nhật schema để sử dụng ví dụ lỗi
                    mediaType.Schema.Example = errorExample;
                    mediaType.Schema.Default = errorExample;
                }
                else
                {
                    // Nếu response chưa được định nghĩa, thêm mới
                    operation.Responses.Add(statusCode, new OpenApiResponse
                    {
                        Description = GetDescriptionForStatusCode(attr.StatusCode),
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            {
                                "application/json",
                                new OpenApiMediaType
                                {
                                    Schema = context.SchemaGenerator.GenerateSchema(typeof(ApiResponse<object>), context.SchemaRepository),
                                    Examples = new Dictionary<string, OpenApiExample>
                                    {
                                        {
                                            "ErrorExample",
                                            new OpenApiExample
                                            {
                                                Value = errorExample,
                                                Summary = GetDescriptionForStatusCode(attr.StatusCode),
                                                Description = "Ví dụ phản hồi lỗi"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });

                    // Định nghĩa schema chính xác
                    var newMediaType = operation.Responses[statusCode].Content["application/json"];
                    newMediaType.Schema.Properties["status"] = new OpenApiSchema { Type = "integer", Format = "int32" };
                    newMediaType.Schema.Properties["data"] = new OpenApiSchema { Type = "object", Nullable = true };
                    newMediaType.Schema.Properties["error"] = new OpenApiSchema { Type = "string", Nullable = true };
                    newMediaType.Schema.Properties["timestamp"] = new OpenApiSchema { Type = "string", Format = "date-time" };

                    // Gán ví dụ lỗi vào schema
                    newMediaType.Schema.Example = errorExample;
                    newMediaType.Schema.Default = errorExample;
                }
            }
        }

        private string GetDescriptionForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => "Error"
            };
        }
    }
}
