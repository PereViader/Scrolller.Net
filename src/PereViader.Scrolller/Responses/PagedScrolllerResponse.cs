namespace PereViader.Scrolller.Responses;

public record PagedScrolllerResponse<T>
{
    public string? Iterator { get; set; }

    public List<T> Items { get; set; } = new List<T>();
}