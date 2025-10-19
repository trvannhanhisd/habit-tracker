namespace HabitTracker.API.Models
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
        public DateTime Timestamp { get; set; }


        public ApiResponse() { }

        // Constructor cho response thành công
        public ApiResponse(T? data, int status = 200)
        {
            Status = status;
            Data = data;
            Timestamp = DateTime.UtcNow;
        }

        // Constructor cho response lỗi
        public ApiResponse(int status, string error)
        {
            Status = status;
            Error = error;
            Timestamp = DateTime.UtcNow;
        }
    }
}
