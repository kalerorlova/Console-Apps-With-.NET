using Dapper;
using Microsoft.Data.SqlClient;
using Spectre.Console;

internal static class AppService {

    internal static void Run() {
        using (var connection = new SqlConnection(AppConfig.serverConnection)) {
            connection.Open();
            connection.Execute($@"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{AppConfig.databaseName}') 
                    CREATE DATABASE {AppConfig.databaseName}");
        }
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            using (var transaction = connection.BeginTransaction()) {
                try {
                    connection.Execute($@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Flashcards')
                            CREATE TABLE Flashcards (
                                    flashcardID INT IDENTITY PRIMARY KEY,
                                    stackName NVARCHAR(100) NOT NULL,
                                    question NVARCHAR(500) NOT NULL,
                                    answer NVARCHAR(1000) NOT NULL)", transaction: transaction);
                    connection.Execute($@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stacks')
                            CREATE TABLE Stacks (
                                    stackID INT IDENTITY PRIMARY KEY,
                                    stackName NVARCHAR(100) UNIQUE NOT NULL)", transaction: transaction);
                    connection.Execute($@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sessions')
                            CREATE TABLE Sessions (
                                    sessionID INT IDENTITY PRIMARY KEY,
                                    stackName NVARCHAR(100),
                                    sessionDate DATETIME NOT NULL,
                                    sessionScore INT NOT NULL DEFAULT 0)", transaction: transaction);
                    connection.Execute(@"IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Flashcards_Stacks')
                            ALTER TABLE Flashcards
                            ADD CONSTRAINT FK_Flashcards_Stacks FOREIGN KEY (stackName) 
                            REFERENCES Stacks (stackName)
                            ON DELETE CASCADE ON UPDATE CASCADE", transaction: transaction);
                    connection.Execute(@"IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Sessions_Stacks')
                            ALTER TABLE Sessions
                            ADD CONSTRAINT FK_Sessions_Stacks FOREIGN KEY (stackName)
                            REFERENCES Stacks (stackName)
                            ON DELETE CASCADE ON UPDATE CASCADE", transaction: transaction);
                    transaction.Commit();
                }
                catch (SqlException) {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        Dictionary<string, Action> mainMenu = new Dictionary<string, Action>();
        mainMenu.Add("1. Manage flashcards", flashcardService.Run);
        mainMenu.Add("2. Manage stacks", stackService.Run);
        mainMenu.Add("3. Manage sessions", sessionService.Run);
        mainMenu.Add("4. Exit the app", delegate() {Console.Clear(); Environment.Exit(0);});
        AnsiConsole.WriteLine("Welcome to the study room!");
        while (true) {
            var menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                .Title("Please select the operation and press ENTER: ")
                                .AddChoices(mainMenu.Keys));
            mainMenu[menuChoice]();
        }
    }
}
