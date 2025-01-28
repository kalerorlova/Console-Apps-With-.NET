
using System.Threading.Tasks;
using Spectre.Console;

internal static class stackController {
    internal static void Create() {
        string name = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the name of the new stack: "));
        int? id = stackRepo.checkExistence(name);
        int count = 5;
        while (id != null && count > 0) {
            name = AnsiConsole.Prompt(new TextPrompt<string>($"A stack {name} already exists! Please enter another name: "));
            id = stackRepo.checkExistence(name);
            count--;
        }
        if (count == 0) {
            AnsiConsole.WriteLine("Maximum number of retries exceeded. Returning back to the menu..");
            return;
        }
        stackRepo.createStack(name);
        Console.Clear();
        AnsiConsole.WriteLine("Stack created successfully!");
    }

    internal static void Update(){
        string updateContent = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                    .Title("What would you like to update?")
                                    .AddChoices("Stack name", "Stack contents (add a new card)"));
        if (updateContent == "Stack name") {
            string orig = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the original name of the stack: "));
            int? origId = stackRepo.checkExistence(orig);
            if (origId == null) {
                AnsiConsole.WriteLine($"Operation invalid: stack {orig} doesn't exist! Returning to the menu..");
                return;
            }
            string name = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the new name of the stack: "));
            int? id = stackRepo.checkExistence(name);
            if (id != null) {
                AnsiConsole.WriteLine($"Operation invalid: stack {name} already exists! Returning to the menu..");
                return;
            }
            stackRepo.updateName(name, origId);
            Console.Clear();
            AnsiConsole.WriteLine("Stack name updated successfully!");
            return;
        }
        else {
            flashcardController.Create();
        }
    }

    internal static void Read() {
        string name = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the name of the stack you'd like to get info about: "));
        int? id = stackRepo.checkExistence(name);
        if (id == null) {
            Console.Clear();
            AnsiConsole.WriteLine($"The stack {name} doesn't exist! Returning to the menu..");
            return;
        }
        List<Flashcard> myList = stackRepo.getStack(name);
        Console.Clear();
        if (myList.Count == 0) {
            AnsiConsole.WriteLine($"The stack {name} has no flashcards!");
            return;
        }
        else {
            StackDTO stack = StackMapper.mapToDTO(name, myList);
            AnsiConsole.WriteLine($"Stack {name} has the following flashcards: ");
            int i = 1;
            foreach (var cardDTO in stack.flashcards) {
                AnsiConsole.WriteLine($"\t{i}. {cardDTO.question}\n\t   {cardDTO.answer}");
                i++;
            }
            AnsiConsole.WriteLine("____________________");
        }
    }

    internal static void ReadAll() {
        List<string> myList = stackRepo.getAll();
        Console.Clear();
        if (myList.Count == 0) {
            AnsiConsole.WriteLine("There are no stacks yet!");
            return;
        }
        AnsiConsole.WriteLine("The available stacks are: ");
        foreach (var stackName in myList) {
            AnsiConsole.WriteLine(stackName);
        }
        AnsiConsole.WriteLine("____________________");
    }

    internal static void Delete() {
        string name = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the name of the new stack: "));
        int? id = stackRepo.checkExistence(name);
        if (id == null) {
            AnsiConsole.WriteLine($"Operation invalid: stack {name} doesn't exist! Returning to the menu..");
            return;
        }
        stackRepo.delete(name);
        Console.Clear();
        AnsiConsole.WriteLine($"Stack {name} deleted successfully!");
    }
}
