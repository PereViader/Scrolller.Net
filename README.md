# Scrolller.Net

Use this library to get any sfw/nsfw images/videos from https://www.scrolller.com
Use responsibly. If you don't use all the assets provided on a single call, prefer to cache the results and reuse them instead of calling scrolller again.

Bindings for microsoft's dependency injection library provided.

Assets returned will be links to photos/videos/gifs


```
var serviceProvider = new ServiceCollection()
    .UseScrolller()
    .BuildServiceProvider();

IScrolllerService scrolllerService = serviceProvider.GetRequiredService<IScrolllerService>()

List<Uri> discoverUris = await scrolllerService.ScrapeDiscover(isNsfw: false); //Use scrolllers home page functionality to get either sfw or nsfw assets

List<Uri> subredditUris = await scrolllerService.ScrapeSubreddit("humor"); //Use scrolllers subreddit functionality to get assets from r/humor
```