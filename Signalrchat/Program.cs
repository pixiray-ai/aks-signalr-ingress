using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Signalrchat;
using System.Net;

namespace Signalrchat
{
    class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                //.UseKestrel(opts =>
                //    {
                //        // Bind directly to a socket handle or Unix socket
                //        // opts.ListenHandle(123554);
                //        // opts.ListenUnixSocket("/tmp/kestrel-test.sock");
                //        opts.Listen(IPAddress.Loopback, port: 5002);
                //        opts.ListenAnyIP(5003);
                //        opts.ListenLocalhost(5004, opts => opts.UseHttps());
                //        opts.ListenLocalhost(5005, opts => opts.UseHttps());
                //    })
                .UseStartup<Startup>();
            });
    }
}

