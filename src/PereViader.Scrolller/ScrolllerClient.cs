using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PereViader.Scrolller;

public class ScrolllerClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ScrolllerClient> _logger;

    public ScrolllerClient(HttpClient httpClient, ILogger<ScrolllerClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Make a request to the scrolller api and return the data as a JsonDocument
    /// </summary>
    /// <exception cref="HttpRequestException">Will be thrown if the response doesn't indicate success</exception>
    /// <exception cref="JsonException">Will occur if the response stream doesn't contain json</exception>
    /// <returns></returns>
    public async Task<JsonDocument> QueryAsync(string request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Making request to scroller with \n{Request}", request);
        
        var requestContent = new StringContent(request, Encoding.Default, "application/json");
        var httpResponse = await _httpClient.PostAsync("api/v2/graphql", requestContent, cancellationToken);
        var dataStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
        var jsonDocument = await JsonDocument.ParseAsync(dataStream, cancellationToken: cancellationToken);
        
        return jsonDocument;
    }
}