[![Run tests](https://github.com/PereViader/Scrolller.Net/actions/workflows/RunTests.yml/badge.svg)](https://github.com/PereViader/Scrolller.Net/actions/workflows/RunTests.yml)

# Scrolller.Net

Use this library to get any sfw/nsfw images/videos from https://www.scrolller.com
Use responsibly. If you don't use all the assets provided on a single call, prefer to cache the results and reuse them instead of calling scrolller again.

Bindings for microsoft's dependency injection library provided.

Assets returned will be links to photos/videos/gifs


```csharp
var serviceProvider = new ServiceCollection()
    .AddScrolller()
    .BuildServiceProvider();

IScrolllerService scrolllerService = serviceProvider.GetRequiredService<IScrolllerService>();


//Use scrolllers discover functionality to get either sfw or nsfw assets
List<Uri> discoverUris = await scrolllerService.Discover(isNsfw: false);


//Use scrolllers subreddit functionality to get assets from the desired subreddit
List<Uri> subredditUris = await scrolllerService.Subreddit("pics"); 


//AsyncEnumerable methods also provided to allow the retrieval of multiple scrolller pages using an iterator to paginate the results internally
var subredditStream = _streamingScrolller.DiscoverStream(isNsfw: false)
    .Take(3); //Take 3 scrolller pages

await foreach (var subreddits in subredditStream)
{
    foreach(var item in subreddits.Items)
    {
        Console.WriteLine(item);
    }
}
```
