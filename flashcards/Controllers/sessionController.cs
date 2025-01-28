using Spectre.Console;

internal static class sessionController {
    internal static void Study() {
        AnsiConsole.WriteLine("Here are all the available flashcard stacks: ");
        stackController.ReadAll();
        string name = AnsiConsole.Prompt(new TextPrompt<string>("Enter the name of the stack you'd like to study: "));
        int? id = stackRepo.checkExistence(name);
        Console.Clear();
        if (id == null) {
            AnsiConsole.WriteLine($"The stack {name} doesn't exist! Returning to the menu..");
            return;
        }
        int score = 0;
        List<Flashcard> cardList = stackRepo.getStack(name);
        DateTime dateNow = DateTime.Now;
        if (cardList.Count == 0) {
            AnsiConsole.WriteLine($"The stack {name} is empty! Congratulations on a useless study session!");
            AnsiConsole.WriteLine($"No logging was done. Returning to the menu..");
            return;
        }
        bool sessionOn = true;
        while (sessionOn) {
            Random rand = new Random();
            int r;
            var menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                        .Title("Would you like to study or end the session? ")
                                        .AddChoices("Get a flashcard to study", "Finish the session"));
            switch (menuChoice) {
                case "Get a flashcard to study":
                    r = rand.Next(0, cardList.Count);
                    string answer = AnsiConsole.Prompt(new TextPrompt<string>($@"
                                    Answer the following question: {cardList[r].question}: "));
                    if (answer == cardList[r].answer) {
                        AnsiConsole.WriteLine("Correct! You get 5 points!");
                        score += 5;
                    }
                    else {
                        AnsiConsole.WriteLine($"The correct answer was {cardList[r].answer}! No points earned this time");
                    }
                    break;
                case "Finish the session":
                    AnsiConsole.WriteLine($"We hope the studying was productive! You have earned {score} points!");
                    sessionOn = false;
                    break;
            }
        }
        sessionRepo.logSession(name, dateNow, score);
        Console.Clear();
        AnsiConsole.WriteLine("The session logged successfully. Returning to the menu..");
        return;  
    }

    internal static void ShowSessions() {
        List<Session> myList = sessionRepo.getAll();
        Console.Clear();
        if (myList.Count == 0) {
            AnsiConsole.WriteLine("No sessions have been logged yet!");
            return;
        }
        //AnsiConsole.WriteLine($"{myList[0].stackName} - {myList[0].date} - {myList[0].score}");
        var table = new Table();
        table.AddColumns("Stack", "Date", "Score");
        table.Centered();
        foreach (var session in myList) {
            table.AddRow(session.stackName, session.sessionDate.ToString("M/d/yyyy"), session.sessionScore.ToString());
        }
        AnsiConsole.Write(table);
    }
        
}
