using System;
using System.Threading.Tasks;

namespace SharpCLI.Samples.DotnetFramework472
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new SharpCliHost("DotnetFramework472CLI", "test cli on DotnetFramework472");

            host.RegisterCommands<Commands>();

            await host.RunAsync(new string[1] { "test" });
        }
    }

    public class Commands
    {
        [Command("test")]
        public static int Test()
        {
            Console.WriteLine("Test");

            return 0;
        }
    }
}