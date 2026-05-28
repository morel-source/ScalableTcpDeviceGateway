using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.Producer;

public static class ServiceCollectionExtensions
{
    public static void AddKafkaProducer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaProducerOptions>(configuration.GetSection(key: KafkaProducerOptions.Section));
        services.AddSingleton<TelemetryKafkaProducer>();
    }
}