using Microsoft.Extensions.DependencyInjection;

namespace OrderProcessor;

public static class ServiceRegistrations
{
    public static void AddOrderProcessorServices(this IServiceCollection services)
    {
        services.AddProcessing();
        services.AddHandlers();
        services.AddEmail();
        services.AddPersistence();
        services.AddMessaging();
    }

    static void AddProcessing(this IServiceCollection services)
    {
        services.AddSingleton<Processing.IProcessor, Processing.PollingProcessor>();
    }

    static void AddHandlers(this IServiceCollection services)
    {
        services.AddTransient<Handlers.ICommandHandler<Commands.CreateCustomer>, Handlers.CommandHandler>();
        services.AddTransient<Handlers.ICommandHandler<Commands.PlaceOrder>, Handlers.CommandHandler>();
    }

    static void AddEmail(this IServiceCollection services)
    {
        services.AddTransient<Email.ISender, Email.Sender>();
    }

    static void AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<Persistence.IAggregateRepository, Persistence.InMemoryAggregateRepository>();
    }

    static void AddMessaging(this IServiceCollection services)
    {
        services.AddSingleton<Messaging.IClient, Messaging.InMemoryQueueClient>();
    }
}