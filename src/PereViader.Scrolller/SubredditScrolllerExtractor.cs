using System.Text.Json;

namespace PereViader.Scrolller;

public class SubredditScrolllerExtractor : IScrolllerExtractor<List<Uri>>
{
    public List<Uri> Extract(JsonDocument jsonDocument)
    {
        var list = new List<Uri>();
        var a = jsonDocument.RootElement
            .GetProperty("data")
            .GetProperty("getSubreddit")
            .GetProperty("children")
            .GetProperty("items");
        foreach (var x in a.EnumerateArray())
        {
            var mediaSources = x.GetProperty("mediaSources");
            var count = mediaSources.GetArrayLength();
            list.Add(new Uri(mediaSources[count - 1].GetProperty("url").GetString()!, UriKind.Absolute));
        }

        return list;
    }
}