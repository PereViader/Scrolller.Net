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
    [TestCase("pics")]
    [TestCase("nsfw")]
    public async Task ScrapeSubredditPosts_ShouldScrapeAll(string subreddit)
    {
        var subredditStream = _streamingScrolller.SubredditStream(subreddit, null, _tokenSource.Token)
            .Take(MAX_PAGES);

        await foreach (var subredditPosts in subredditStream)
        {
            Assert.That(subredditPosts.Items, Is.Not.Empty);
        }
    }
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task ScrapeDiscover_ShouldScrapeAll(bool isNsfw)
    {
        var subredditStream = _streamingScrolller.DiscoverStream(isNsfw, null, _tokenSource.Token)
            .Take(MAX_PAGES);

        await foreach (var subreddits in subredditStream)
        {
            Assert.That(subreddits.Items, Is.Not.Empty);
        }
    }
}