using Microsoft.Extensions.Logging;

namespace PereViader.Scrolller;

public sealed class ScrolllerService : IScrolllerService
{
    private readonly ScrolllerClient _scrolllerClient;
    private readonly DiscoverScrolllerExtractor _discoverExtractor;
    private readonly SubredditScrolllerExtractor _subredditExtractor;
    private readonly ILogger<ScrolllerService> _logger;

    public ScrolllerService(ScrolllerClient scrolllerClient, 
        DiscoverScrolllerExtractor discoverExtractor, 
        SubredditScrolllerExtractor subredditExtractor,
        ILogger<ScrolllerService> logger)
    {
        _scrolllerClient = scrolllerClient;
        _discoverExtractor = discoverExtractor;
        _subredditExtractor = subredditExtractor;
        _logger = logger;
    }
    
    public Task<List<Uri>> Subreddit(string subreddit, string? iterator = null, CancellationToken ctx = default)
    {
        iterator = string.IsNullOrWhiteSpace(iterator) ? "null" : $"\"{iterator}\"";

        _logger.LogDebug("Requesting subreddit data for {Sub}", subreddit);

        var request = $$"""
            {
                "query": " query SubredditQuery( $url: String! $filter: SubredditPostFilter $iterator: String ) { getSubreddit(url: $url) { children( limit: 50 iterator: $iterator filter: $filter disabledHosts: null ) { iterator items { __typename id url title subredditId subredditTitle subredditUrl redditPath isNsfw albumUrl hasAudio fullLengthSource gfycatSource redgifsSource ownerAvatar username displayName isPaid isFavorite mediaSources { url width height isOptimized } blurredMediaSources { url width height isOptimized } } } } } ",
                "variables": {
                    "url": "/r/{{subreddit}}",
                    "filter": null,
                    "hostsDown": null,
                    "iterator": {{iterator}}
                },
                "authorization": null
            }
            """;

        return Scrape(request, _subredditExtractor, ctx);
    }

    public Task<List<Uri>> Discover(bool isNsfw, string? iterator = null, CancellationToken ctx = default)
    {
        iterator = string.IsNullOrWhiteSpace(iterator) ? "null" : $"\"{iterator}\"";

        _logger.LogDebug("Requesting discover data; isNsfw: {Val}", isNsfw);
        var request = $$"""
            {
                "query": " query DiscoverSubredditsQuery( $filter: MediaFilter $limit: Int $iterator: String ) { discoverSubreddits( isNsfw: {{isNsfw.ToString().ToLowerInvariant()}} filter: $filter limit: $limit iterator: $iterator ) { iterator items { __typename id url title secondaryTitle description createdAt isNsfw subscribers isComplete itemCount videoCount pictureCount albumCount isPaid username tags banner { url width height isOptimized } isFollowing children( limit: 2 iterator: null filter: null disabledHosts: null ) { iterator items { __typename id url title subredditId subredditTitle subredditUrl redditPath isNsfw albumUrl hasAudio fullLengthSource gfycatSource redgifsSource ownerAvatar username displayName isPaid isFavorite mediaSources { url width height isOptimized } blurredMediaSources { url width height isOptimized } } } } } } ",
                "variables": {
                    "limit": 30,
                    "filter": null,
                    "hostsDown": null,
                    "iterator": {{iterator}}
                },
                "authorization": null
            }
            """;

        return Scrape(request, _discoverExtractor, ctx);
    }

    private async Task<List<Uri>> Scrape(string request, IScrolllerExtractor<List<Uri>> extractor, CancellationToken cancellationToken = default)
    {
        using var jsonDocument = await _scrolllerClient.QueryAsync(request, cancellationToken);
        var data = extractor.Extract(jsonDocument);
        return data;
    }
}