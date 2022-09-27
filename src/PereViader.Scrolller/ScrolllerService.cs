using System.Net;
using System.Text;
using System.Text.Json;

namespace PereViader.Scrolller;

public sealed class ScrolllerService : IScrolllerService
{
    public Task<List<Uri>> Subreddit(string subreddit)
    {
        var request = $$"""
            {
                "query": " query SubredditQuery( $url: String! $filter: SubredditPostFilter $iterator: String ) { getSubreddit(url: $url) { children( limit: 50 iterator: $iterator filter: $filter disabledHosts: null ) { iterator items { __typename id url title subredditId subredditTitle subredditUrl redditPath isNsfw albumUrl hasAudio fullLengthSource gfycatSource redgifsSource ownerAvatar username displayName isPaid isFavorite mediaSources { url width height isOptimized } blurredMediaSources { url width height isOptimized } } } } } ",
                "variables": {
                    "url": "/r/{{subreddit}}",
                    "filter": null,
                    "hostsDown": null
                },
                "authorization": null
            }
            """;

        return Scrape(request, SubredditScrolllerExtractor.Instance);
    }

    public Task<List<Uri>> Discover(bool isNsfw)
    {
        var request = $$"""
            {
                "query": " query DiscoverSubredditsQuery( $filter: MediaFilter $limit: Int $iterator: String ) { discoverSubreddits( isNsfw: {{isNsfw.ToString().ToLowerInvariant()}} filter: $filter limit: $limit iterator: $iterator ) { iterator items { __typename id url title secondaryTitle description createdAt isNsfw subscribers isComplete itemCount videoCount pictureCount albumCount isPaid username tags banner { url width height isOptimized } isFollowing children( limit: 2 iterator: null filter: null disabledHosts: null ) { iterator items { __typename id url title subredditId subredditTitle subredditUrl redditPath isNsfw albumUrl hasAudio fullLengthSource gfycatSource redgifsSource ownerAvatar username displayName isPaid isFavorite mediaSources { url width height isOptimized } blurredMediaSources { url width height isOptimized } } } } } } ",
                "variables": {
                    "limit": 30,
                    "filter": null,
                    "hostsDown": null
                },
                "authorization": null
            }
            """;

        return Scrape(request, DiscoverScrolllerExtractor.Instance);
    }

    public async Task<T> Scrape<T>(string request, IScrolllerExtractor<T> extractor)
    {
        var handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
        };
        
        using var httpClient = new HttpClient(handler);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.scrolller.com/api/v2/graphql");

        requestMessage.Headers.Add("Accept-Encoding", "gzip, deflate, br");
        requestMessage.Headers.Add("accept", "*/*");

        requestMessage.Content = new StringContent(request, Encoding.Default, "application/json");

        var httpResponse = await httpClient.SendAsync(requestMessage);

        var content = await httpResponse.Content.ReadAsStringAsync();

        var jsonDocument = JsonDocument.Parse(content);

        return extractor.Extract(jsonDocument);
    }
}