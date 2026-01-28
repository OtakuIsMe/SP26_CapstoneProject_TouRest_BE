namespace TouRest.Api.Common
{
    public class ApiResponse<T>
    {
        public int Code { get; init; }
        public string Message { get; init; } = default!;
        public T? Data { get; init; }
    }
}
