namespace PereViader.Scrolller;

public interface IScrolllerService
{
    Task<List<Uri>> Discover(bool isNsfw, string? iterator = null, CancellationToken ctx = default);
    Task<List<Uri>> Subreddit(string subreddit, string? iterator = null, CancellationToken ctx = default);
}