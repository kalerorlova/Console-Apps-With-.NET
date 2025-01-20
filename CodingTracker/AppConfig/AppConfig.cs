using Microsoft.Extensions.Configuration;

internal static class AppConfig {

        public static string connectionString {get; private set; }

        static AppConfig() {
            IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            connectionString = config.GetConnectionString("DefaultConnection") ?? "";
        }

}
