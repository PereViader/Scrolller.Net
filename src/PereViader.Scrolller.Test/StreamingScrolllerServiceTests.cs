using Microsoft.Extensions.DependencyInjection;

namespace PereViader.Scrolller.Test;

public class StreamingScrolllerServiceTests
{
    private const int MAX_PAGES = 3;
    private IScrolllerService _streamingScrolller;
    private CancellationTokenSource _tokenSource;
    
    [SetUp]
    public void Setup()
    {
        var serviceProvider = new ServiceCollection()
            .AddScrolller()
            .BuildServiceProvider();
        _tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

        _streamingScrolller = serviceProvider.GetRequiredService<IScrolllerService>();
    }

    [Test]
    [TestCase("humor")]
    [TestCase("nsfw")]
    public async Task ScrapeSubredditPosts_ShouldScrapeAll(string subreddit)
    {
        var pageNum = 0;
        var subredditStream = _streamingScrolller.SubredditStream(subreddit, null, _tokenSource.Token);
        await foreach (var subredditPosts in subredditStream)
        {
            if (string.IsNullOrWhiteSpace(subredditPosts.Iterator))
            {
                Assert.That(subredditPosts.Items, Is.Empty);
            }
            else
            {
                Assert.That(subredditPosts.Items, Is.Not.Empty);
            }
            
            pageNum++;
            if (pageNum >= MAX_PAGES)
                break;
        }
        
        Assert.That(pageNum, Is.GreaterThan(0));
    }
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task ScrapeDiscover_ShouldScrapeAll(bool isNsfw)
    {
        var pageNum = 0;
        var subredditStream = _streamingScrolller.DiscoverStream(isNsfw, null, _tokenSource.Token);
        await foreach (var subreddits in subredditStream)
        {
            if (string.IsNullOrWhiteSpace(subreddits.Iterator))
            {
                Assert.That(subreddits.Items, Is.Empty);
            }
            else
            {
                Assert.That(subreddits.Items, Is.Not.Empty);
            }
            
            pageNum++;
            if (pageNum >= MAX_PAGES)
                break;
        }
        
        Assert.That(pageNum, Is.GreaterThan(0));
    }
}