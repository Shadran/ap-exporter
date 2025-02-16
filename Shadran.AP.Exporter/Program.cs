using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Shadran.Ap.Services;
using Shadran.AP.Exporter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole();
builder.Services.AddHttpClient();

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("shadran.ap", "shadran", "1.0.0", false, builder.Configuration["OpenTelemetry:InstanceId"]))
    .WithMetrics(o =>
    {
        o
        .AddMeter("shadran.ap.trackers")
        .AddOtlpExporter(e =>
        {
            e.Endpoint = new Uri($"{builder.Configuration["OpenTelemetry:Endpoint"]!}/v1/metrics");
            e.Headers = $"Authorization=Basic {builder.Configuration["OpenTelemetry:ApiToken"]}";
            e.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
        });
    });

builder.Services.AddSingleton<APTrackerReader>();
builder.Services.AddSingleton<TrackerMetricsService>();
builder.Services.Configure<ExporterOptions>(o =>
{
    o.TrackerId = builder.Configuration["Archipelago:TrackerId"]!;
    o.PollingSeconds = int.Parse(builder.Configuration["Archipelago:PollingSeconds"]!);
    o.Filters.Games = builder.Configuration.GetSection("Archipelago:Filters:Games")?.Get<string[]>() ?? [];
    o.Filters.Players = builder.Configuration.GetSection("Archipelago:Filters:Players")?.Get<string[]>() ?? [];
});
builder.Services.AddHostedService<ExporterService>();


using IHost host = builder.Build();

await host.RunAsync();