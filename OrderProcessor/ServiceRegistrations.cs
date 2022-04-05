using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using OrderProcessor.Domain;
using Serilog;

namespace OrderProcessor;

public static class ServiceRegistrations
{
    public static void AddOrderProcessorServices(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddTime();
        services.AddProcessing();
        services.AddHandlers();
        services.AddEmail();
        services.AddPersistence();
        services.AddMessaging();
    }

    static void AddLogging(this IServiceCollection services)
    {
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        services.AddSingleton<ILogger>(logger);
    }

    static void AddTime(this IServiceCollection services)
    {
        services.AddSingleton<IClock>(SystemClock.Instance);
    }

    static void AddProcessing(this IServiceCollection services)
    {
        services.AddSingleton<Processing.IProcessor, Processing.PollingProcessor>();
    }

    static void AddHandlers(this IServiceCollection services)
    {
        services.AddTransient<Handlers.ICommandHandler<Commands.CreateCustomer>, Handlers.CreateCustomerHandler>();
        services.Decorate<Handlers.ICommandHandler<Commands.CreateCustomer>>((inner, provider) => 
            new Handlers.LoggingHandlerDecorator<Commands.CreateCustomer>(inner, provider.GetRequiredService<ILogger>()));

        services.AddTransient<Handlers.ICommandHandler<Commands.PlaceOrder>, Handlers.PlaceOrderHandler>();
        services.Decorate<Handlers.ICommandHandler<Commands.PlaceOrder>>((inner, provider) =>
            new Handlers.LoggingHandlerDecorator<Commands.PlaceOrder>(inner, provider.GetRequiredService<ILogger>()));
    }

    static void AddEmail(this IServiceCollection services)
    {
        services.AddTransient<Email.ISender, Email.Sender>();
        services.AddTransient<Email.IComposer, Email.Composer>();
    }

    static void AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<Persistence.IAggregateRepository<CustomerId, Customer>, Persistence.InMemoryAggregateRepository<CustomerId, Customer, CustomerState>>();
        services.AddSingleton<Func<CustomerId, CustomerState, Customer>>(provider => (id, state) => new Customer(
            id,
            state,
            provider.GetService<IClock>()!));
    }

    static void AddMessaging(this IServiceCollection services)
    {
        services.AddSingleton<Messaging.IClient, Messaging.InMemoryQueueClient>();
    }
}