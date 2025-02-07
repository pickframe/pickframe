using Application.Services;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Kafka;

[ExcludeFromCodeCoverage]
public class KafkaProducerService : IEnqueuService
{
    private readonly IProducer<string, string> _producer;
    private readonly string topic;
    

    public KafkaProducerService(IConfiguration configuration)
    {
        var bootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers");
        topic = configuration.GetValue<string>("Kafka:Topic")!;

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        CreateTopicAsync(bootstrapServers!, topic).Wait();

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public static async Task CreateTopicAsync(string bootstrapServers, string topicName, int numPartitions = 1, short replicationFactor = 1)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        }).Build();

        try
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));

            // Verificar se o tópico já existe
            if (metadata.Topics.Any(t => t.Topic == topicName))
            {
                Console.WriteLine($"Tópico '{topicName}' já existe.");
                return;
            }

            // Criar o tópico
            await adminClient.CreateTopicsAsync(
            [
                new TopicSpecification
                {
                    Name = topicName,
                    NumPartitions = numPartitions,
                    ReplicationFactor = replicationFactor
                }
            ]);

            Console.WriteLine($"Tópico '{topicName}' criado com sucesso.");
        }
        catch (CreateTopicsException ex)
        {
            Console.WriteLine($"Erro ao criar o tópico '{topicName}': {ex.Results[0].Error.Reason}");
        }
    }

    public async Task EnqueueProcess(string processId)
    {
        try
        {
            var deliveryReport = await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = processId,
                Value = processId
            });

            Console.WriteLine($"Mensagem enviada para a partição {deliveryReport.Partition}, offset {deliveryReport.Offset}");
        }
        catch (ProduceException<string, string> ex)
        {
            Console.WriteLine($"Erro ao enviar mensagem: {ex.Error.Reason}");
        }
    }
}
