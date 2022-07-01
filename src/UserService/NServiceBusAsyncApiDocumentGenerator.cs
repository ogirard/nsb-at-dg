using System.Reflection;
using NJsonSchema.Generation;
using NServiceBus;
using Saunter.AsyncApiSchema.v2;
using Saunter.Generation;
using Saunter.Generation.Filters;
using Saunter.Generation.SchemaGeneration;
using Saunter.Utils;
using IMessage = NServiceBus.IMessage;

namespace UserService;

internal sealed class NServiceBusAsyncApiDocumentGenerator : IDocumentGenerator
{
    public AsyncApiDocument GenerateDocument(TypeInfo[] asyncApiTypes, Saunter.AsyncApiOptions options, AsyncApiDocument prototype, IServiceProvider serviceProvider)
    {
        var asyncApiSchema = prototype.Clone();

        var schemaResolver = new AsyncApiSchemaResolver(asyncApiSchema, options.JsonSchemaGeneratorSettings);

        var generator = new JsonSchemaGenerator(options.JsonSchemaGeneratorSettings);
        asyncApiSchema.Channels = GenerateChannels(schemaResolver, generator);

        var filterContext = new DocumentFilterContext(asyncApiTypes, schemaResolver, generator);
        foreach (var filterType in options.DocumentFilters)
        {
            var filter = (IDocumentFilter)serviceProvider.GetRequiredService(filterType);
            filter.Apply(asyncApiSchema, filterContext);
        }

        return asyncApiSchema;
    }

    private IDictionary<string, ChannelItem> GenerateChannels(AsyncApiSchemaResolver schemaResolver, JsonSchemaGenerator schemaGenerator)
    {
        var channels = new Dictionary<string, ChannelItem>();
        var messageContracts = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
                a.GetTypes().Where(t => t.IsAssignableTo(typeof(IMessage)))).ToArray();
        channels.AddRange(GenerateEventChannels(
            messageContracts.Where(x => x.IsAssignableTo(typeof(IEvent)) && x != typeof(IEvent)), schemaResolver, schemaGenerator));
        channels.AddRange(GenerateCommandChannels(
            messageContracts.Where(x => x.IsAssignableTo(typeof(ICommand)) && x != typeof(ICommand)), schemaResolver, schemaGenerator));
        return channels;
    }

    private IDictionary<string, ChannelItem> GenerateEventChannels(IEnumerable<Type> eventTypes, AsyncApiSchemaResolver schemaResolver, JsonSchemaGenerator schemaGenerator)
    {
        var publishChannels = new Dictionary<string, ChannelItem>();
        foreach (var eventType in eventTypes)
        {
            // how to know which are the events that are owned by the UserService? Only these should be included here

            var operationId = eventType.FullName ?? Guid.NewGuid().ToString("D");
            var subscribeOperation = new Operation
            {
                OperationId = operationId,
                Summary = string.Empty,
                Description = string.Empty,
                Message = GenerateMessageFromType(eventType, schemaResolver, schemaGenerator),
                Bindings = null,
            };

            var channelItem = new ChannelItem
            {
                Description = eventType.FullName,
                Parameters = new Dictionary<string, IParameter>(),
                Publish = null,
                Subscribe = subscribeOperation,
                Bindings = null,
                Servers = null,
            };

            publishChannels.Add(operationId, channelItem);
        }

        return publishChannels;
    }

    private IDictionary<string, ChannelItem> GenerateCommandChannels(IEnumerable<Type> commandTypes, AsyncApiSchemaResolver schemaResolver, JsonSchemaGenerator schemaGenerator)
    {
        var subscribeChannels = new Dictionary<string, ChannelItem>();
        foreach (var commandType in commandTypes)
        {
            // how to know which are the events that are owned by the UserService? Only these should be included here

            var operationId = commandType.FullName ?? Guid.NewGuid().ToString("D");

            var publishOperation = new Operation
            {
                OperationId = operationId,
                Summary = string.Empty,
                Description = string.Empty,
                Message = GenerateMessageFromType(commandType, schemaResolver, schemaGenerator),
                Bindings = null,
            };

            var channelItem = new ChannelItem
            {
                Description = commandType.FullName,
                Parameters = new Dictionary<string, IParameter>(),
                Publish = publishOperation,
                Subscribe = null,
                Bindings = null,
                Servers = null,
            };

            subscribeChannels.Add(operationId, channelItem);
        }

        return subscribeChannels;
    }

    private static Saunter.AsyncApiSchema.v2.IMessage GenerateMessageFromType(Type payloadType, AsyncApiSchemaResolver schemaResolver, JsonSchemaGenerator jsonSchemaGenerator)
    {
        var message = new Message
        {
            Payload = jsonSchemaGenerator.Generate(payloadType, schemaResolver),
            Name = payloadType.FullName
        };

        return schemaResolver.GetMessageOrReference(message);
    }
}