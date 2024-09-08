using MassTransit;
using Voyago.Auth.BusinessLogic;
using Voyago.Auth.BusinessLogic.Config;
using Voyago.Auth.BusinessLogic.Extensions;
using Voyago.Auth.DataAccessLayer.Common;
using Voyago.Auth.DataAccessLayer.Extensions;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager config = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices().AddDALServices(config["ConnectionStrings:Database"]!);
#region JWT
builder.Services.AddSingleton<IJWTConfig>(_ =>
                            new JWTConfig(config["JWT:Audience"]!,
                                          config["JWT:Issuer"]!,
                                          config["JWT:HashingKey"]!));
#endregion

#region MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<IBusinessLogicMarker>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(config["RabbitMQ:Host"], "/", h =>
        {
            h.Username(config["RabbitMQ:Username"]!);
            h.Password(config["RabbitMQ:Password"]!);
        });

        // Configure consumers
        cfg.ConfigureEndpoints(context);
    });
});
#endregion
WebApplication app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
#region DB Initialization 

IDbInitializer dbInitializer = app.Services.GetRequiredService<IDbInitializer>();
await dbInitializer.InitializeAsync();
#endregion
app.Run();
