using System.Reflection;
using AmazonS3;
using Application.Services;
using Domain.Entities.Process;
using Kafka;
using Microsoft.AspNetCore.Http.Features;
using Postgresql;
using Postgresql.Repositories;
using Serilog;

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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

// Postgres
builder.Services.AddTransient<DbSession>();
builder.Services.AddTransient<IProcessRepository, ProcessRepository>();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
