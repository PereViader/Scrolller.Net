using System.Text.Json.Serialization;

namespace PereViader.Scrolller;

public record GraphApiRequest(string Query)
{
    public Dictionary<string, object?> Variables { get; } = new()
    {
        ["Filter"] = null,
        ["Iterator"] = null,
        ["HostsDown"] = null
    };
    
    public string? Authorization { get; init; }
}