using Microsoft.Extensions.DependencyInjection;

namespace PereViader.Scrolller.Test;

public class StreamingScrolllerServiceTests
{
    private IStreamingScrolllerService _streamingScrolller;
    private CancellationTokenSource _tokenSource;
    
    [SetUp]
    public void Setup()
    {
        var serviceProvider = new ServiceCollection()
            .AddScrolller()
            .BuildServiceProvider();
        _tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

        _streamingScrolller = serviceProvider.GetRequiredService<IStreamingScrolllerService>();
    }
    
    [Test]
    [TestCase("humor")]
    [TestCase("nsfw")]
    public async Task ScrapeSubreddit_ShouldScrapeSomeUrls(string subreddit)
    {
        const int MAX_URLS = 100;
        var assets = new List<Uri>();
        try
        {
            await foreach (var imageUrl in _streamingScrolller.SubredditUrls(subreddit, _tokenSource.Token))
            {
                assets.Add(imageUrl);
                Assert.That(imageUrl.Scheme, Is.EqualTo(Uri.UriSchemeHttps));
                
                if (assets.Count >= MAX_URLS)
                {
                    _tokenSource.Cancel();
                }
            }
        }
        catch (OperationCanceledException)
        {
            Assert.That(assets.Count, Is.EqualTo(MAX_URLS));
        }

        Assert.That(assets.Any(), Is.True);
    }
    
    [Test]
    [TestCase("humor")]
    [TestCase("nsfw")]
    public async Task ScrapeSubredditPosts_ShouldScrapeAll(string subreddit)
    {
        var MAX_COUNT = 60;
        var subredditStream = _streamingScrolller.Subreddit(subreddit, _tokenSource.Token);
        var totalCount = 0;
        await foreach (var subredditPosts in subredditStream)
        {
            var postCount = subredditPosts.Count();
            Assert.That(postCount, Is.GreaterThan(0));
            totalCount += postCount;
            if (totalCount >= 60)
                return;
        }
    }
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task ScrapeDiscover_ShouldScrapeAll(bool isNsfw)
    {
        var MAX_ITEMS = 60;
        var numItems = 0;
        try
        {
            var subredditStream = _streamingScrolller.Discover(isNsfw, _tokenSource.Token);
            await foreach (var subreddits in subredditStream)
            {
                var subredditCount = subreddits.Count();
                Assert.That(subredditCount, Is.GreaterThan(0));
                numItems += subredditCount;

                if (numItems >= MAX_ITEMS)
                    _tokenSource.Cancel();
            }
        }
        catch (OperationCanceledException)
        {
        }
        
        Assert.That(numItems, Is.GreaterThan(0));
    }
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task ScrapeDiscoverUrls_ShouldScrapeAll(bool isNsfw)
    {
        var MAX_ITEMS = 100;
        var numItems = 0;
        try
        {
            var discoverUrls = _streamingScrolller.DiscoverUrls(isNsfw, _tokenSource.Token);
            await foreach (var url in discoverUrls)
            {
                Assert.That(url.Scheme, Is.EqualTo(Uri.UriSchemeHttps));
                numItems++;
                if (numItems >= MAX_ITEMS)
                {
                    _tokenSource.Cancel();
                }
            }
        }
        catch (OperationCanceledException)
        {
        }

        Assert.That(numItems, Is.GreaterThan(0));
    }
}