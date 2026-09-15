namespace ShowcaseProject.Shared.Model.Wrapper
{
    public class ServiceWrapper<T> where T : class
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public string? DetailedErrorMessage { get; init; }
        public ServiceErrorKind ErrorKind { get; init; }
    }
}
