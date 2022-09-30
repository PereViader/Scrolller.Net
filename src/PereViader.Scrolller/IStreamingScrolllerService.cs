using PereViader.Scrolller.Responses;

namespace PereViader.Scrolller;

public interface IStreamingScrolllerService
{
    IAsyncEnumerable<IEnumerable<SubredditPostResponse>> Subreddit(string subreddit, CancellationToken ctx = default);
    IAsyncEnumerable<IEnumerable<SubredditResponse>> Discover(bool isNsfw, CancellationToken ctx = default);
    IAsyncEnumerable<Uri> DiscoverUrls(bool isNsfw, CancellationToken ctx = default);
    IAsyncEnumerable<Uri> SubredditUrls(string subreddit, CancellationToken ctx = default);
}