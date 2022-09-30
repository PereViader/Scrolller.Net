using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PereViader.Scrolller;

public class StreamingScrolllerService : IStreamingScrolllerService
{
    private readonly ScrolllerClient _client;
    private readonly ILogger<StreamingScrolllerService> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    
    public StreamingScrolllerService(ScrolllerClient client, ILogger<StreamingScrolllerService> logger)
    {
        _client = client;
        _logger = logger;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }


    public async IAsyncEnumerable<IEnumerable<SubredditPostResponse>> Subreddit(string subreddit, [EnumeratorCancellation] CancellationToken ctx)
    {
        string? iterator = null;

        do
        {
            iterator = string.IsNullOrWhiteSpace(iterator) ? "null" : $"\"{iterator}\"";
            
            var request = $$"""
            {
                "query": "query SubredditQuery( $url: String! $filter: SubredditPostFilter $iterator: String ) { getSubreddit(url: $url) { children( limit: 50 iterator: $iterator filter: $filter disabledHosts: null ) { iterator items { __typename id url title subredditId subredditTitle subredditUrl redditPath isNsfw albumUrl hasAudio fullLengthSource gfycatSource redgifsSource ownerAvatar username displayName isPaid isFavorite mediaSources { url width height isOptimized } blurredMediaSources { url width height isOptimized } } } } } ",
                "variables": {
                    "url": "/r/{{ subreddit}}  ",
                    "filter": null,
                    "hostsDown": null,
                    "iterator": {{iterator}}
                },
                "authorization": null
            }
            """ ;

            using var jsonDocument = await _client.QueryAsync(request, ctx);
            var pagedResponse = jsonDocument.RootElement
                .GetProperty("data")
                .GetProperty("getSubreddit")
                .GetProperty("children")
                .Deserialize<PagedScrolllerResponse<SubredditPostResponse>>(_jsonSerializerOptions);

            iterator = pagedResponse?.Iterator;
            yield return pagedResponse?.Items ?? Enumerable.Empty<SubredditPostResponse>();
        } while (!string.IsNullOrWhiteSpace(iterator) || ctx.IsCancellationRequested);
    }
    
    public async IAsyncEnumerable<IEnumerable<SubredditResponse>> Discover(bool isNsfw, [EnumeratorCancellation] CancellationToken ctx)
    {
        string? iterator = null;

        do
        {
            iterator = string.IsNullOrWhiteSpace(iterator) ? "null" : $"\"{iterator}\"";
                
            var request = $$"""
            {
                "query": " query DiscoverSubredditsQuery( $filter: MediaFilter $limit: Int $iterator: String ) { discoverSubreddits( isNsfw: {{isNsfw.ToString().ToLowerInvariant()}} filter: $filter limit: $limit iterator: $iterator ) { iterator items { __typename id url title secondaryTitle description createdAt isNsfw subscribers isComplete itemCount videoCount pictureCount albumCount isPaid username tags banner { url width height isOptimized } isFollowing children( limit: 15 iterator: null filter: null disabledHosts: null ) { iterator items { __typename id url title subredditId subredditTitle subredditUrl redditPath isNsfw albumUrl hasAudio fullLengthSource gfycatSource redgifsSource ownerAvatar username displayName isPaid isFavorite mediaSources { url width height isOptimized } blurredMediaSources { url width height isOptimized } } } } } } ",
                "variables": {
                    "limit": 30,
                    "filter": null,
                    "hostsDown": null,
                    "iterator": {{iterator}}
                },
                "authorization": null
            }
            """;

            using var jsonDocument = await _client.QueryAsync(request, ctx);
            
            var pagedResponse = jsonDocument.RootElement
                .GetProperty("data")
                .GetProperty("discoverSubreddits")
                .Deserialize<PagedScrolllerResponse<SubredditResponse>>(_jsonSerializerOptions);
            
            iterator = pagedResponse?.Iterator;
            yield return pagedResponse?.Items ?? Enumerable.Empty<SubredditResponse>();

        } while (!string.IsNullOrWhiteSpace(iterator) || ctx.IsCancellationRequested);
    }

    public async IAsyncEnumerable<Uri> DiscoverUrls(bool isNsfw, [EnumeratorCancellation] CancellationToken ctx = default)
    {
        await foreach (var discoverResponse in Discover(isNsfw, ctx))
        {
            var subredditPosts = discoverResponse.SelectMany(dr => dr.Children.Items);
            foreach (var uri in GetImageUrls(subredditPosts))
            {
                yield return uri;
            }
        }
    }

    public async IAsyncEnumerable<Uri> SubredditUrls(string subreddit, [EnumeratorCancellation] CancellationToken ctx = default)
    {
        await foreach (var subredditPosts in Subreddit(subreddit, ctx))
        {
            foreach (var uri in GetImageUrls(subredditPosts))
            {
                yield return uri;
            }
        }
    }
    
    private IEnumerable<Uri> GetImageUrls(IEnumerable<SubredditPostResponse> subredditPosts)
    {
        foreach (var subredditPost in subredditPosts)
        {
            var mediaInfo = subredditPost.MediaSources.Last();
            _logger.LogDebug("From {Post} returning {Media}",subredditPost, mediaInfo);
            yield return new Uri(mediaInfo.Url);
        }
    }
}