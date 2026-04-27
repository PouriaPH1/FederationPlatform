namespace FederationPlatform.Application.DTOs;

public class OperationResultDto
{
    public bool Succeeded { get; set; }
    public List<string> Messages { get; set; } = new();
    public object? Data { get; set; }
}

public class OperationResultDto<T> where T : class
{
    public bool Succeeded { get; set; }
    public List<string> Messages { get; set; } = new();
    public T? Data { get; set; }
}
