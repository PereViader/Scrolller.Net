namespace PereViader.Scrolller;

public record PagedScrolllerResponse<T>
{
    public string? Iterator { get; set; }

    public List<T> Items { get; set; } = new List<T>();
}