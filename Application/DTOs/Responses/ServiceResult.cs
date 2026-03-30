namespace Application.DTOs.Responses
{
    public record ServiceResult(string Code, string Message)
    {
        public static ServiceResult Success(string message) => new("Success", message);
        public static ServiceResult Validation(string message) => new("ValidationError", message);
        public static ServiceResult NotFound(string message) => new("NotFound", message);
        public static ServiceResult Forbidden(string message) => new("Forbidden", message);
        public static ServiceResult Conflict(string message) => new("Conflict", message);
        public static ServiceResult Error(string message) => new("ServerError", message);
    }
}
