using Microsoft.Extensions.DependencyInjection;

namespace PereViader.Scrolller.Test;

public class TestScrolllerService
{
    IScrolllerService? scrolllerService;

    [SetUp]
    public void Setup()
    {
        var serviceProvider = new ServiceCollection()
            .AddScrolller()
            .BuildServiceProvider();

        scrolllerService = serviceProvider.GetRequiredService<IScrolllerService>();
    }

    [Test]
    [TestCase("nsfw")]
    [TestCase("humor")]
    public async Task ScrapeSubreddit_ShouldScrapeSomeUrls(string subreddit)
    {
        var assets = await scrolllerService!.Subreddit(subreddit);
        Assert.That(assets.Items, Is.Not.Empty);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task ScrapeDiscover_ShouldScrapeSomeUrls(bool isNsfw)
    {
        var assets = await scrolllerService!.Discover(isNsfw);
        Assert.That(assets.Items, Is.Not.Empty);
    }
}
