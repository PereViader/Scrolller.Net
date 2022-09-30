using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace PereViader.Scrolller;

public static class ServiceExtensions
{
    private const DecompressionMethods AUTOMATIC_DECOMPRESSION = DecompressionMethods.GZip 
                                                                  | DecompressionMethods.Deflate 
                                                                  | DecompressionMethods.Brotli;

    public static IServiceCollection AddScrolller(this IServiceCollection serviceCollection)
    {

        serviceCollection.AddHttpClient<ScrolllerClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://api.scrolller.com");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("accept", "*/*");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler
                {
                    AutomaticDecompression = AUTOMATIC_DECOMPRESSION
                };

                return handler;
            });
        
        serviceCollection.AddTransient<IScrolllerService, ScrolllerService>();
        
        return serviceCollection;
    }
}