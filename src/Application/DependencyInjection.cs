using System.Reflection;
using Application.Abstractions.Behaviors;
using Application.Abstractions.Messaging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));
        // Decorate only when implementations exist. Apply inner-first (validation), then outer (logging).
        // Decorate closed generics individually to avoid Scrutor.Decorate open-generic failures.
        // Apply inner decorators first (validation), then outer (logging).
        //DecorateClosedGenerics(services, typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        //DecorateClosedGenerics(services, typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

        //DecorateClosedGenerics(services, typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        //DecorateClosedGenerics(services, typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        //DecorateClosedGenerics(services, typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
    //private static void DecorateClosedGenerics(IServiceCollection services, Type openServiceType, Type openDecoratorType)
    //{
        
    //    if (!openServiceType.IsGenericTypeDefinition)
    //        throw new ArgumentException("openServiceType must be an open generic type", nameof(openServiceType));
    //    if (!openDecoratorType.IsGenericTypeDefinition)
    //        throw new ArgumentException("openDecoratorType must be an open generic type", nameof(openDecoratorType));

    //    // Capture the closed generic service types that match the open generic
    //    var closedServiceTypes = services
    //        .Where(sd => sd.ServiceType is not null
    //                     && sd.ServiceType.IsGenericType
    //                     && sd.ServiceType.GetGenericTypeDefinition() == openServiceType)
    //        .Select(sd => sd.ServiceType)
    //        .Distinct()
    //        .ToList();

    //    foreach (var closedService in closedServiceTypes)
    //    {
    //        var typeArgs = closedService.GetGenericArguments();
    //        var closedDecorator = openDecoratorType.MakeGenericType(typeArgs);

    //        // Decorate the concrete closed generic service
    //        services.Decorate(closedService, closedDecorator);
    //    }
    //}
}
