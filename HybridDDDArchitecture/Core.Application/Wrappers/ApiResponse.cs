namespace HybridDDDArchitecture.Core.Application.Wrappers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new();

        // Constructor para respuestas exitosas
        public ApiResponse(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        // Constructor para respuestas con errores
        public ApiResponse(bool success, string message, T data, List<string> errors)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
        }

        // Constructor para respuestas de error sin data
        public ApiResponse(bool success, string message)
        {
            Success = success;
            Message = message;
            Data = default(T);
            Errors = new List<string>();
        }
    }
}
/*namespace HybridDDDArchitecture.Core.Application.Wrappers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public ApiResponse(bool success, string message, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public ApiResponse(bool success, string message, T? data = default, List<string>? errors = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
        }
    }
}*/