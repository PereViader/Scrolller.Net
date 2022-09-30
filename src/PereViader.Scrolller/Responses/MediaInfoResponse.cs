namespace PereViader.Scrolller;

public record MediaInfoResponse
{
    public string Url { get; set; }
    public long Height { get; set; }
    public long Width { get; set; }
    public bool IsOptimized { get; set; }
}