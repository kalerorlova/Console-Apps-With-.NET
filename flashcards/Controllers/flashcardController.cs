using Spectre.Console;

internal static class flashcardController {
    internal static void Create() {
        string stackName = AnsiConsole.Prompt(new TextPrompt<string>(
                "Please enter the name of the stack to which you'd like to add a flashcard: "));
        int? id = stackRepo.checkExistence(stackName);
        if (id == null) {
            AnsiConsole.WriteLine("The current stack does not exist! Returning to the menu..");
            return;
        }
        string question = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the question (front of the flashcard): "));
        string answer = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the answer (back of the flashcard): "));
        flashcardRepo.create(stackName, question, answer);
        Console.Clear();
        AnsiConsole.WriteLine($"Flashcard in the stack {stackName} created successfully!");
    }

    internal static void Update() {
        string updateContent = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                    .Title("What would you like to update?")
                                    .AddChoices("Flashcard question", "Flashcard answer", "Move the card to another stack"));
        int tryId = AnsiConsole.Prompt(new TextPrompt<int>("Please enter the id of the flashcard you'd like to update: "));
        int? id = flashcardRepo.checkExistence(tryId);
        if (id == null) {
            AnsiConsole.WriteLine("A card with the given ID doesn't exist! Returning to the menu..");
            return;
        }
        switch (updateContent) {
            case "Flashcard question":
                string question = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the question (front of the flashcard): "));
                flashcardRepo.updateInfo("question", question, id);
                break;
            case "Flashcard answer":
                string answer = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the answer (back of the flashcard): "));
                flashcardRepo.updateInfo("answer", answer, id);
                break;
            case "Move the card to another stack":
                string stackName = AnsiConsole.Prompt(new TextPrompt<string>(
                            "Please enter the name of the stack to which you'd like to move the flashcard: "));
                int? stackID = stackRepo.checkExistence(stackName);
                if (stackID == null) {
                    AnsiConsole.WriteLine("The current stack does not exist! Returning to the menu..");
                    return;
                }
                flashcardRepo.updateInfo(stackID, id);
                break;
        }
    }

    internal static void Read() {
        int id = AnsiConsole.Prompt(new TextPrompt<int>("Please enter the id of the flashcard you'd like to view: "));
        Flashcard? card = flashcardRepo.getCard(id);
        Console.Clear();
        if (card == null) {
            AnsiConsole.WriteLine("A card with the given ID doesn't exist! Returning to the menu..");
            return;
        }
        FlashcardDTO cardDTO = FlashcardMapper.mapToDTO(card);
        AnsiConsole.WriteLine("Retrieved the card successfully: ");
        AnsiConsole.WriteLine(cardDTO.question + "\n" + cardDTO.answer);
    }

    internal static void Delete() {
        int tryId = AnsiConsole.Prompt(new TextPrompt<int>("Please enter the id of the flashcard you'd like to delete: "));
        int? id = flashcardRepo.checkExistence(tryId);
        if (id == null) {
            AnsiConsole.WriteLine("A card with the given ID doesn't exist! Returning to the menu..");
            return;
        }
        flashcardRepo.delete(id);
        Console.Clear();
        AnsiConsole.WriteLine("Flashcard deleted successfully!");
    }
}
