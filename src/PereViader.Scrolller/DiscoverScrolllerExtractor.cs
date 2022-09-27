using System.Text.Json;

namespace PereViader.Scrolller;

public class DiscoverScrolllerExtractor : IScrolllerExtractor<List<Uri>>
{
    public static DiscoverScrolllerExtractor Instance { get; } = new DiscoverScrolllerExtractor();

    public List<Uri> Extract(JsonDocument jsonDocument)
    {
        var list = new List<Uri>();
        var a = jsonDocument.RootElement
            .GetProperty("data")
            .GetProperty("discoverSubreddits")
            .GetProperty("items");
        foreach (var x in a.EnumerateArray())
        {
            foreach (var y in x.GetProperty("children").GetProperty("items").EnumerateArray())
            {
                var mediaSources = y.GetProperty("mediaSources");
                var count = mediaSources.GetArrayLength();
                list.Add(new Uri(mediaSources[count - 1].GetProperty("url").GetString()!, UriKind.Absolute));
            }
        }

        return list;
    }
}