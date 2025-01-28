using System.Dynamic;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;

internal static class flashcardService {

        static Dictionary<string, Action> flashcardMenu = new Dictionary<string, Action>() {
            {"1. Create a flashcard and add it to an existing stack", flashcardController.Create},
            {"2. Update a flashcard", flashcardController.Update},
            {"3. Get info about a flashcard", flashcardController.Read},
            {"4. Delete a flashcard", flashcardController.Delete},
            {"5. Return to the main menu", delegate () {return;}}
        };

        internal static void Run() {
            Console.Clear();
            var menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                        .Title("Choose an operation: ")
                                        .AddChoices(flashcardMenu.Keys));
            flashcardMenu[menuChoice]();
        } 
}