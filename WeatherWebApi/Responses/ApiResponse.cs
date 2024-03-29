namespace WeatherWebApi.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; }

        public ApiResponse(T data, string message = "")
        {
            Success = true;
            Message = message;
            Data = data;
            Errors = new List<string>();
        }

        public ApiResponse(List<string> errors, string message = "")
        {
            Success = false;
            Message = message;
            Errors = errors;
            Data = default;
        }
    }
}
