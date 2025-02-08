using AmazonS3;
using Application.Services;
using Database;
using FFMpeg;
using Kafka;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace API;

[ExcludeFromCodeCoverage]
#pragma warning disable CS1591
public static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/pickframe-log-.txt", rollingInterval: RollingInterval.Day)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Host.UseSerilog();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

        builder.Services.AddInfraData(builder.Configuration);

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = builder.Configuration.GetValue<long>("FilLengthLimit");
        });

        // Amazon
        builder.Services.AddSingleton<IStorageService, AmazonS3Service>();

        // Kafka
        builder.Services.AddSingleton<IEnqueuService, KafkaProducerService>();
        builder.Services.AddSingleton<KafkaConsumerService>();
        builder.Services.AddHostedService<KafkaBackgroundService>();

        // FFMpeg
        builder.Services.AddSingleton<IVideoProcessorService, FFMpegProcessor>();

        var app = builder.Build();

        app.UseSerilogRequestLogging();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        using (var serviceScope = app.Services.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
            dbContext.Database.Migrate();
        }

        app.Run();
    }
}