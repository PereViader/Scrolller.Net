namespace PereViader.Scrolller;

public interface IScrolllerService
{
    Task<List<Uri>> Discover(bool isNsfw);
    Task<List<Uri>> Subreddit(string subreddit);
    Task<T> Scrape<T>(string request, IScrolllerExtractor<T> extractor);
}