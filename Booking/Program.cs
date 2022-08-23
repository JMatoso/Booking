using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Booking.Data;
using Booking.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry(options =>
{
    options.Dsn = "";
    options.Debug = true;
    options.TracesSampleRate = 1.0;

    options.BeforeSend = @event =>
    {
        // Never report server names
        @event.ServerName = null;
        return @event;
    };
});

builder.Host
    .UseMetricsWebTracking()
    .UseMetrics(options =>
    {
        options.EndpointOptions = endpointsOptions =>
        {
            endpointsOptions.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
            endpointsOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
            endpointsOptions.EnvironmentInfoEndpointEnabled = false;
        };
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    });

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddMetrics()
    .AddLogging()
    .AddScoped<IBookRepository, BookRepository>()
    .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSentryTracing();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
