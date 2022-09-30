namespace PereViader.Scrolller.Responses;

public record MediaInfoResponse
{
    public string Url { get; init; }
    public long Height { get; init; }
    public long Width { get; init; }
    public bool IsOptimized { get; init; }
}