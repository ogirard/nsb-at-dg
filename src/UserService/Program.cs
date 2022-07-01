using NServiceBus;
using Saunter;
using Saunter.AsyncApiSchema.v2;
using Saunter.Generation;
using UserService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IDocumentGenerator, NServiceBusAsyncApiDocumentGenerator>();
builder.Services.AddAsyncApiSchemaGeneration(options =>
{
    options.AsyncApi = new AsyncApiDocument
    {
        Info = new Info("UserService API", "1.0.0")
        {
            Description = "Some example.",
            License = new License("Apache 2.0")
            {
                Url = "https://www.apache.org/licenses/LICENSE-2.0"
            }
        },
        Servers = { { "amqp", new Server("sb://example.servicebus.windows.net/", "amqp") } }
    };

});

builder.Host.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("UserService");
    var transport = endpointConfiguration.UseTransport<LearningTransport>();
    return endpointConfiguration;
});


var app = builder.Build();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapAsyncApiDocuments();
        endpoints.MapAsyncApiUi();
    });
}

app.MapControllers();

app.Run();
