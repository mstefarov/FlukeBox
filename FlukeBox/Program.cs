using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace FlukeBox {
    public class Program {
        public static void Main(string[] args) {
            var currentDirectory = Directory.GetCurrentDirectory();
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .UseContentRoot(currentDirectory)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        public static string Version { get; } = "0.1.0";
    }
}
