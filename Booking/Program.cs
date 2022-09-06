using Serilog;
using Booking.Data;
using Booking.Extensions;
using Booking.Repository;
using App.Metrics.AspNetCore;
using Microsoft.EntityFrameworkCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Booking.Options;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = Environments.Development
});

var sentryOptions = new SentryOptions();

builder.Configuration.Bind(nameof(SentryOptions), sentryOptions);
builder.Services.AddSingleton(sentryOptions);

builder.WebHost.UseSentry(options =>
{
    options.Debug = sentryOptions.Debug;
    options.TracesSampleRate = sentryOptions.TracesSampleRate;
    options.Dsn = sentryOptions.Dsn;

    options.MaxRequestBodySize = sentryOptions.MaxRequestBodySize;
    options.SendDefaultPii = sentryOptions.SendDefaultPii;
    options.MinimumBreadcrumbLevel = sentryOptions.MinimumBreadcrumbLevel;
    options.MinimumEventLevel = sentryOptions.MinimumEventLevel; 
    options.DiagnosticLevel = sentryOptions.DiagnosticsLevel;
    options.AttachStacktrace = sentryOptions.AttachStackTrace;

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
    .UseSerilog(Logging.ConfigureLogger)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    });

// Add services to the container.
builder.Services.AddScoped<ValidationFilterAttribute>();

builder.Services.Configure<ApiBehaviorOptions>(opt
    => opt.SuppressModelStateInvalidFilter = true);

builder.Services.AddControllers(opt =>
{
    opt.RespectBrowserAcceptHeader = true;
    opt.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc($"v1", new OpenApiInfo
    {
        Title = "Booking API",
        Version = $"v1",
        Description = "Monitoring, health checks and error tracking in a simple Book API.",
        Contact = new OpenApiContact
        {
            Name = "José Matoso",
            Email = "jos3matosoj@gmail.com",
            Url = new Uri("https://github.com/JMatoso/Booking")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://github.com/JMatoso/Booking/blob/master/LICENSE.txt")
        },
        TermsOfService = new Uri("https://github.com/JMatoso/Booking")
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

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

app.UseCors(opt => {
    opt.AllowAnyHeader();
    opt.AllowAnyHeader();
    opt.SetIsOriginAllowed((host) => true);
    opt.AllowCredentials();
});

app.MapControllers();

app.Run();
