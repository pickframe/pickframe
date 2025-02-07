using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Kafka;

[ExcludeFromCodeCoverage]
public class KafkaBackgroundService : BackgroundService
{
    private readonly KafkaConsumerService _consumerService;

    public KafkaBackgroundService(KafkaConsumerService consumerService)
    {
        _consumerService = consumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(() => _consumerService.StartConsuming(stoppingToken), stoppingToken);
    }
}
