using System;
using Spectre.Console;
using Dapper;
using Microsoft.Data.Sqlite;


internal class AppController(CodingSessionController cdController) {

    public void Run() {
        using (var connection = new SqliteConnection(AppConfig.connectionString)) {
        connection.Open();
        connection.Execute(@"CREATE TABLE IF NOT EXISTS Code_Sessions (
            id          INTEGER PRIMARY KEY AUTOINCREMENT,
            startTime   TEXT NOT NULL,
            endTime     TEXT NOT NULL,
            duration    TEXT NOT NULL
        )");
        connection.Close();
        }
        bool appOn = true;
        Dictionary<string, Action> menu = new Dictionary<string, Action>();
        menu.Add("1. Record a new session", cdController.createSession);
        menu.Add("2. Record a new session using stopwatch", cdController.createTimedSession);
        menu.Add("3. Get all sessions info", cdController.readSessions);
        menu.Add("4. Update the session info", cdController.updateSession);
        menu.Add("5. Delete a session", cdController.deleteSession);
        menu.Add("6. Filter sessions by period", cdController.filterSessions);
        menu.Add("7. Run statistics summary by period", cdController.runSummary);
        menu.Add("6. Exit", delegate() {
            appOn = false;
        });
        AnsiConsole.WriteLine("Welcome to the Coding Tracker!");
        while (appOn) {
            var menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Please select the operation of your choice and press ENTER: ")
                .AddChoices(menu.Keys)
            );
            menu[menuChoice]();
        }
    }
}
