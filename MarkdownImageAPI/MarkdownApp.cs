using MarkdownImageAPI.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MarkdownImageAPI;

public class MarkdownApp
{
    private static HostApplicationBuilder _hostApplicationBuilder = Host.CreateApplicationBuilder();

    public static IHost? IHost { get; private set; } = null;

    public static void Start()
    {
        _hostApplicationBuilder.Services.AddHostedService<HttpServer>();
        IHost = _hostApplicationBuilder.Build();
        IHost.Run();
    }
}
