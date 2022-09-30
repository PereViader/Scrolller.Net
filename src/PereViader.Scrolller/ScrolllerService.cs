using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PereViader.Scrolller.Responses;

namespace PereViader.Scrolller;

public sealed class ScrolllerService : IScrolllerService
{
    private readonly ScrolllerClient _scrolllerClient;
    private readonly ILogger<ScrolllerService> _logger;
    private readonly JsonSerializerOptions _serializerOptions;

    public ScrolllerService(ScrolllerClient scrolllerClient, ILogger<ScrolllerService> logger)
    {
        _scrolllerClient = scrolllerClient;
        _logger = logger;
        _serializerOptions = new JsonSerializerOptions
        {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async IAsyncEnumerable<PagedScrolllerResponse<SubredditPostResponse>> SubredditStream(
        string subreddit,
        string? iterator = null,
        [EnumeratorCancellation] CancellationToken ctx = default)
    {
        do
        {
            var postResponses = await Subreddit(subreddit, iterator, ctx);
            iterator = postResponses.Iterator;
            yield return postResponses;
        } while (!string.IsNullOrWhiteSpace(iterator));
    }

    public async IAsyncEnumerable<PagedScrolllerResponse<SubredditResponse>> DiscoverStream(
        bool isNsfw, 
        string? iterator = null, 
        [EnumeratorCancellation] CancellationToken ctx = default)
    {
        do
        {
            var subredditResponse = await Discover(isNsfw, iterator, ctx);
            iterator = subredditResponse.Iterator;
            yield return subredditResponse;
        } while (!string.IsNullOrWhiteSpace(iterator));
    }

    public async Task<PagedScrolllerResponse<SubredditPostResponse>> Subreddit(string subreddit, string? iterator = null, CancellationToken ctx = default)
    {
        const string QUERY = $$"""query SubredditQuery( $url: String! $filter: SubredditPostFilter $iterator: String ) { getSubreddit(url: $url) { children( limit: 50 iterator: $iterator filter: $filter disabledHosts: null ) { iterator items { __typename id url title subredditId subredditTitle subredditUrl redditPath isNsfw albumUrl hasAudio fullLengthSource gfycatSource redgifsSource ownerAvatar username displayName isPaid isFavorite mediaSources { url width height isOptimized } blurredMediaSources { url width height isOptimized } } } } }""";

        var request = new GraphApiRequest(QUERY);
        request.Variables["Iterator"] = iterator;
        request.Variables["Url"] = $"/r/{subreddit}";

        using var jsonDocument = await _scrolllerClient.QueryAsync(request, ctx);
        
        var pagedSubreddits = jsonDocument.RootElement
            .GetProperty("data")
            .GetProperty("getSubreddit")
            .GetProperty("children")
            .Deserialize<PagedScrolllerResponse<SubredditPostResponse>>(_serializerOptions);
        
        return pagedSubreddits ?? new PagedScrolllerResponse<SubredditPostResponse>();
    }
    
    public async Task<PagedScrolllerResponse<SubredditResponse>> Discover(bool isNsfw, string? iterator = null, CancellationToken ctx = default)
    {
        _logger.LogDebug("Requesting discover data; isNsfw: {Val}", isNsfw);
        var query = $$"""query DiscoverSubredditsQuery( $filter: MediaFilter $limit: Int $iterator: String ) { discoverSubreddits( isNsfw: {{isNsfw.ToString().ToLowerInvariant()}} filter: $filter limit: $limit iterator: $iterator ) { iterator items { __typename id url title secondaryTitle description createdAt isNsfw subscribers isComplete itemCount videoCount pictureCount albumCount isPaid username tags banner { url width height isOptimized } isFollowing children( limit: 5 iterator: null filter: null disabledHosts: null ) { iterator items { __typename id url title subredditId subredditTitle subredditUrl redditPath isNsfw albumUrl hasAudio fullLengthSource gfycatSource redgifsSource ownerAvatar username displayName isPaid isFavorite mediaSources { url width height isOptimized } blurredMediaSources { url width height isOptimized } } } } } }""";

        var request = new GraphApiRequest(query);
        request.Variables["Iterator"] = iterator;
        request.Variables["Limit"] = 30;

        using var jsonDocument = await _scrolllerClient.QueryAsync(request, ctx);

        var pagedSubreddits = jsonDocument.RootElement
            .GetProperty("data")
            .GetProperty("discoverSubreddits")
            .Deserialize<PagedScrolllerResponse<SubredditResponse>>(_serializerOptions);
        
        return pagedSubreddits ?? new PagedScrolllerResponse<SubredditResponse>();
    }
}