using Microsoft.Extensions.Hosting;

namespace Kafka;

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
