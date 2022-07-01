using System.Reflection;
using NServiceBus;
using OrderService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Host.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("OrderService");
    var transport = endpointConfiguration.UseTransport<LearningTransport>();
    
    var allCommandTypes = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

    foreach (var commandType in allCommandTypes)
    {
        var endpoint = commandType.GetCustomAttribute<DestinationAttribute>()?.EndpointName;
        var isInternal = commandType.GetCustomAttribute<DestinationAttribute>()?.IsInternal;

        if (endpoint != null)
        {
            if (isInternal != null && !isInternal.Value)
            {
                transport.Routing().RouteToEndpoint(commandType, endpoint);
            }
        }
        else
        {
            throw new InvalidOperationException(
                $"{nameof(DestinationAttribute.EndpointName)} is not specified for command {commandType.Name}");
        }
    }

    return endpointConfiguration;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
