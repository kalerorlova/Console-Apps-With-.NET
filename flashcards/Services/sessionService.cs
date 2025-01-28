using Spectre.Console;

internal static class sessionService {

    static Dictionary<string, Action> sessionMenu = new Dictionary<string, Action>() {
        {"1. Begin a study session", sessionController.Study},
        {"2. Show all study sessions", sessionController.ShowSessions}
    };

    internal static void Run() {
        Console.Clear();
        var menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                        .Title("Choose an operation: ")
                                        .AddChoices(sessionMenu.Keys));
            sessionMenu[menuChoice]();
    }

}
