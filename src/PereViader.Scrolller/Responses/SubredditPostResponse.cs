namespace PereViader.Scrolller;

public record SubredditPostResponse
{
    public long Id { get; set; }
    public string? RedditPath { get; set; }
    public string? Url { get; set; }
    public string? Title { get; set; }
    public List<MediaInfoResponse> MediaSources { get; set; }
}