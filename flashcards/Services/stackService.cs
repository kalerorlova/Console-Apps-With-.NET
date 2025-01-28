using Spectre.Console;

internal static class stackService {
    static Dictionary<string, Action> stackMenu = new Dictionary<string, Action>() {
            {"1. Create a new stack", stackController.Create},
            {"2. Update a stack", stackController.Update},
            {"3. Get a stack's contents", stackController.Read},
            {"4. Get all stacks", stackController.ReadAll},
            {"5. Delete a stack", stackController.Delete},
            {"6. Return to the main menu", delegate () {return;}}
        };

        internal static void Run() {
            Console.Clear();
            var menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                        .Title("Choose an operation: ")
                                        .AddChoices(stackMenu.Keys));
            stackMenu[menuChoice]();
        } 
        
}
