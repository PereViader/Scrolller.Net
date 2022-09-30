namespace PereViader.Scrolller;

public record SubredditResponse
{
    public long Id { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public bool IsNsfw { get; set; }
    public string CreatedAt { get; set; }
    public PagedScrolllerResponse<SubredditPostResponse> Children { get; set; }
}