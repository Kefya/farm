namespace FarmGame.Api.DTOs.Common
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
        public static OperationResult Fail(string message) => new() { Success = false, Message = message };
    }
}