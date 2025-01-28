using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

internal static class AppConfig {
        public static string? serverConnection {get; private set; }
        public static string? databaseName {get; private set; }
        public static string? databaseConnection {get; private set; }

        public static void Run() {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

            databaseConnection = config.GetConnectionString("DefaultConnection") ?? "";
            Regex regex = new Regex ("Database=[a-zA-Z0-9-_]+");
            Match match = regex.Match(databaseConnection);
            if (!match.Success) {
                AnsiConsole.WriteLine("The database name can contain only alphanumeric symbols, '-', and '_'!");
                throw new Exception("Please choose a valid name for the database. Exiting the program..");
            }
            databaseName = match.ToString();
            databaseName = Regex.Replace(databaseName, "Database=", "");
            serverConnection = Regex.Replace(databaseConnection, "Database=[a-zA-Z0-9-_]+;", "");
        }
}
