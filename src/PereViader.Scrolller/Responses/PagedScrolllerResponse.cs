namespace PereViader.Scrolller.Responses;

public record PagedScrolllerResponse<T>
{
    public string? Iterator { get; init; }

    public List<T> Items { get; init; } = new();
}