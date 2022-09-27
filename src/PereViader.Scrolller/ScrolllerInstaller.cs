using Microsoft.Extensions.DependencyInjection;

namespace PereViader.Scrolller;

public static class ScrolllerInstaller
{
    public static IServiceCollection UseScrolller(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IScrolllerService, ScrolllerService>();
        return serviceCollection;
    }
}