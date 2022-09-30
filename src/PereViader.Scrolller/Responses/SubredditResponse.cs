namespace PereViader.Scrolller.Responses;

public record SubredditResponse
{
    public long Id { get; init; }
    public string Url { get; init; }
    public string Title { get; init; }
    public bool IsNsfw { get; init; }
    public string CreatedAt { get; init; }
    public PagedScrolllerResponse<SubredditPostResponse> Children { get; init; }
}