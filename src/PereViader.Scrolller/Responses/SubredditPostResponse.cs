namespace PereViader.Scrolller.Responses;

public record SubredditPostResponse
{
    public long Id { get; init; }
    public string? RedditPath { get; init; }
    public string? Url { get; init; }
    public string? Title { get; init; }
    public List<MediaInfoResponse> MediaSources { get; init; }
}