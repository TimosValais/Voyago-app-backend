using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
using System.Text;
using Voyago.App.Api.Extensions;
using Voyago.App.BusinessLogic;
using Voyago.App.BusinessLogic.Exceptions;
using Voyago.App.BusinessLogic.Extensions;
using Voyago.App.DataAccessLayer.Common;
using Voyago.App.DataAccessLayer.Extensions;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager config = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();

#region Logging
//builder.Logging.ClearProviders();
//builder.Logging.AddJsonConsole();

#endregion
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer abcdef12345\"",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddApplicationServices().AddDALServices(config["ConnectionStrings:Database"]!);
#region JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["JWT:Issuer"],
            ValidAudience = config["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:HashingKey"]!)),

        };
    });

builder.Services.AddAuthorization();
#endregion
#region Minio


builder.Services.AddMinio(opts =>
{
    string endpoint = config["Minio:Endpoint"]!;
    string accessKey = config["Minio:AccessKey"]!;
    string secretKey = config["Minio:SecretKey"]!;
    opts.WithEndpoint(endpoint)
        .WithCredentials(accessKey, secretKey);
});
//builder.Services.AddSingleton(sp =>
//{


//    return new MinioClient()
//        .WithEndpoint(endpoint)
//        .WithCredentials(accessKey, secretKey);
//    //.WithSSL(); // Enable SSL if using HTTPS
//});
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

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policyBuilder =>
    {
        policyBuilder
            .WithOrigins(config["Frontend:Url"]!)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
#endregion
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("FrontendPolicy");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//error handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next(); // Continue processing the request
    }
    catch (ConfictException conEx)
    {
        context.Response.StatusCode = 409; // Conflict status code
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync($"Conflict occurred: {conEx.Message}");
    }
    catch (Exception ex)
    {

        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync($"An error occurred: {ex.Message}");
    }
});
app.UseTaskRequestMiddleware();
app.MapControllers();

#region DB Initialization 

IDbInitializer dbInitializer = app.Services.GetRequiredService<IDbInitializer>();


try
{
    await dbInitializer.InitializeAsync();

}
catch (Exception)
{
}
#endregion

app.Run();
