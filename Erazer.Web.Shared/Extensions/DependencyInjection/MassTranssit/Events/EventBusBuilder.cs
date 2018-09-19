﻿using System;
using Erazer.Infrastructure.ServiceBus;
using Erazer.Messages.IntegrationEvents.Infrastructure;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Erazer.Web.Shared.Extensions.DependencyInjection.MassTranssit.Events
{

    /// <summary>
    /// EventBusBuilder builder Interface
    /// </summary>
    public interface IEventBusBuilder
    {
        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        IServiceCollection Services { get; }

        /// <summary>
        /// Gets the bus settings 
        /// </summary>
        ServiceBusSettings Settings { get; }
    }

    public class EventBusBuilder : IEventBusBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventBusBuilder"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="settings">The settings of the bus</param>
        /// <exception cref="System.ArgumentNullException">services</exception>
        public EventBusBuilder(IServiceCollection services, ServiceBusSettings settings)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Settings = settings;
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        /// <inheritdoc />
        public ServiceBusSettings Settings { get; }
    }

    public static class EventBusBuilderExtensions
    {
        /// <summary>
        /// Adds the EventPublisher service, this is used for 'publishing' events on the bus
        /// </summary>
        /// <param name="builder">The builder</param>
        /// <returns></returns>
        public static IEventBusBuilder AddEventPublisher(this IEventBusBuilder builder)
        {
            builder.Services.AddScoped<IIntegrationEventPublisher, EventPublisher>();
            return builder;
        }

        /// <summary>
        /// Adds events listeners
        /// This will startup a background 'job' that listens to the specifc event types and 'publishes' it with Mediatr.
        /// </summary>
        /// <param name="configure">Method to configure the queue name and the event types for consuming</param>
        /// <param name="builder">The builder</param>
        /// <returns></returns>
        public static IEventBusBuilder AddEventListeners(this IEventBusBuilder builder, Action<EventServiceCollectionConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var configurator = new ServiceCollectionConfigurator(builder.Services);
            var wrapper = new EventServiceCollectionConfigurator(configurator);

            configure(wrapper);

            builder.Services.AddSingleton(configurator);
            builder.Services.AddSingleton(provider => EventBusFactory.Build(cfg =>
            {
                var host = cfg.Host(new Uri(builder.Settings.ConnectionString), h =>
                {
                    h.Username(builder.Settings.UserName);
                    h.Password(builder.Settings.Password);
                });

                cfg.ReceiveEndpoint(host, wrapper.EventQueueName, e =>
                {
                    e.LoadFrom(provider);
                });
            }));

            builder.Services.AddSingleton<IHostedService, ServiceBusEventHost>();
            return builder;
        }
    }
}
