using System.Reflection;
using Microsoft.OpenApi.Models;
using Serilog.Extensions.Logging;
using Serilog;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

namespace Signalrchat;

public class Startup
{
    public IConfiguration Configuration { get; }
    readonly string MyAllowSpecificOrigins = "SignalRPolicy";


    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// This method gets called by the runtime.
    /// Use this method to add services to the container.
    /// 
    /// 2. services.AddSignalR() adds the SignalR service to the application's service collection.
    /// This service is necessary to enable SignalR functionality in your application, including real-time communication between clients and the server.
    ///
    /// 3. services.AddCors() is used to configure Cross-Origin Resource Sharing (CORS) in your application.
    /// CORS is a security mechanism that restricts browser-based requests to a different origin (domain, protocol, or port) than the one from which the request originated.
    /// In this case, the CORS policy allows requests from http://localhost:5000 origin.
    ///
    /// 4. o.AddDefaultPolicy() specifies the default CORS policy for the application.
    /// The code inside the lambda expression configures the policy by allowing any HTTP method, any HTTP header, and allowing credentials (such as cookies) to be sent with the request.
    ///
    /// b.WithOrigins("http://localhost:5000") specifies the allowed origin for the CORS policy.
    /// In this case, it allows requests from http://localhost:5000 to access the server.
    ///
    /// By configuring the CORS policy in this way, you are allowing requests from http://localhost:5000 to access your server,
    /// which is important when using SignalR for real-time communication between clients and the server.
    ///
    /// NOTE on CORS: This is a sample code:
    /// See Documentation https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-7.0 and
    /// Github Sample https://github.com/dotnet/AspNetCore.Docs/blob/live/aspnetcore/security/cors/8.0sample/Cors/Web2API/Program.cs
    /// for more information. 
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        // Configure Serilog
        var logger = new LoggerConfiguration()
            .WriteTo.Console() // Configure Serilog sinks as per your requirements
            .CreateLogger();

        //KNOWN PROXIES
        string[] ips = new[] { "127.0.0.1" };
        var envProxies = Environment.GetEnvironmentVariable("KNOWN_PROXIES");
        logger.Information("KNOWN_PROXIES: {KNOWN_PROXIES}", envProxies);
        if (!string.IsNullOrEmpty(envProxies))
        {
            ips = envProxies.Split(";");
        }

        //Configure forwarded headers and known proxies
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            foreach (var ip in ips)
            {
                options.KnownProxies.Add(IPAddress.Parse(ip));
                logger.Information("Information Adding proxy: {ip}", IPAddress.Parse(ip));
            }
        });

        services.AddControllers();
        services.AddRazorPages();
        services.AddSignalR();
        // We want to use Serilog for logging instead of the default logger.

        // Configure logging for the application
        services.AddLogging(builder =>
        {
            builder.AddSerilog(logger);
        });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        // set the CORS policy - endpointroting with named policy 
        string[] origins = new[] { "https://localhost", "https://localhost:8080", "http://localhost:5000" };

        var corsOriginsEnvVar = Environment.GetEnvironmentVariable("CORS_ORIGINS");
        if (!string.IsNullOrEmpty(corsOriginsEnvVar))
        {
            origins = corsOriginsEnvVar.Split(";");
        }
        logger.Information("CORS_ORIGINS: {CORS_ORIGINS}", origins);
        
        services.AddCors(o =>
        {
            //o.AddDefaultPolicy(b =>
            //{
            //    b.AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials()
            //        .WithOrigins(origins);
            //});
            o.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {
                    policy.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });
        
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddProvider(new SignalRLoggerProvider());

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });


        app.UseHttpsRedirection();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Client Backend");
        });

        app.UseFileServer();

        app.UseRouting();

        app.UseAuthorization();
        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            //endpoints.MapControllers().RequireCors(MyAllowSpecificOrigins);
            //endpoints.MapHub<ChatHub>("/chatHub");
            endpoints.MapHub<ChatHub>("/chatHub").RequireCors(MyAllowSpecificOrigins);
            endpoints.MapRazorPages();
        });
    }
}