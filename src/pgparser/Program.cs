using System;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace pgparser
{
    [Command("pgparse", Description = "PGP Parser/Converter")]
    [Subcommand(ParseCommand.Name, typeof(ParseCommand))]
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Any(a => string.Equals(a, "--debug")))
            {
                args = args.Where(a => !string.Equals(a, "--debug")).ToArray();
                Console.WriteLine($"Waiting for debugger, Process ID: {System.Diagnostics.Process.GetCurrentProcess().Id}");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();
            }

            return CommandLineApplication.Execute<Program>(args);
        }

        public int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return 0;
        }
    }
}
