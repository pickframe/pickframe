using System.Threading.Tasks;
using Application.UseCases.GetFrames;
using Confluent.Kafka;
using Domain.Entities.Process;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka;

public class KafkaConsumerService
{
    private readonly string _topic;
    private readonly IConsumer<string, string> _consumer;
    private readonly IServiceScopeFactory _scopedFactory;

    public KafkaConsumerService(
        IConfiguration configuration,
        IServiceScopeFactory scopedFactory)
    {
        _scopedFactory = scopedFactory;
        _topic = configuration.GetValue<string>("Kafka:Topic")!;

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers"),
            GroupId = configuration.GetValue<string>("Kafka:GroupId"),
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    public async Task StartConsuming(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                Console.WriteLine($"Mensagem recebida: {consumeResult.Message.Value}");

                using (var scope = _scopedFactory.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    GetFramesRequest getFramesRequest = new() { Id = consumeResult.Message.Value };
                    await mediator.Send(getFramesRequest, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Consumo cancelado.");
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            _consumer.Close();
        }
    }
}
