using PereViader.Scrolller.Responses;

namespace PereViader.Scrolller;

public interface IScrolllerService
{
    Task<PagedScrolllerResponse<SubredditResponse>> Discover(bool isNsfw, string? iterator = null, CancellationToken ctx = default);
    Task<PagedScrolllerResponse<SubredditPostResponse>> Subreddit(string subreddit, string? iterator = null, CancellationToken ctx = default);
    IAsyncEnumerable<PagedScrolllerResponse<SubredditPostResponse>> SubredditStream(string subreddit, string? iterator = null, CancellationToken ctx = default);
    IAsyncEnumerable<PagedScrolllerResponse<SubredditResponse>> DiscoverStream(bool isNsfw, string? iterator = null, CancellationToken ctx = default);
}