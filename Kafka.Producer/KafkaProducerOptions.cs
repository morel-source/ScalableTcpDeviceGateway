namespace Kafka.Producer;

public sealed class KafkaProducerOptions
{
    public const string Section = "Kafka";

    public required string BootstrapServers { get; set; }
    public int MaxRetries { get; set; } = 3;
}