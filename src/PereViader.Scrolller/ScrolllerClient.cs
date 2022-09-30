using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PereViader.Scrolller;

public class ScrolllerClient
{

    private const int MIN_RESPONSES = 5;
    private const int MAX_RESPONSES = 50;
    
    private readonly HttpClient _httpClient;
    private readonly ILogger<ScrolllerClient> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ScrolllerClient(HttpClient httpClient, ILogger<ScrolllerClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    /// <summary>
    /// Make a request to the scrolller api and return the data as a JsonDocument
    /// </summary>
    /// <exception cref="HttpRequestException">Will be thrown if the response doesn't indicate success</exception>
    /// <exception cref="JsonException">Will occur if the response stream doesn't contain json</exception>
    /// <returns></returns>
    public async Task<JsonDocument> QueryAsync(GraphApiRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Making request to scroller with \n{Request}", request);

        if (request.Variables.TryGetValue("Limit", out var limitVal))
        {
            var limit = limitVal as int? ?? MAX_RESPONSES;
            request.Variables["Limit"] = Math.Clamp(limit, MIN_RESPONSES, MAX_RESPONSES);
        }
        
        using var httpResponse = await _httpClient.PostAsJsonAsync("api/v2/graphql", request, _jsonSerializerOptions, cancellationToken);
        var dataStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        return await JsonDocument.ParseAsync(dataStream, cancellationToken: cancellationToken);
    }
}