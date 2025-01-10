using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;

List<string> history = new List<string>();

int menuChoice = -1;
DifficultyLevel currDiff = DifficultyLevel.Easy;
int score = 0;
bool randomMode = false;
bool gameOn = true;

while (gameOn) {
    randomMode = false;
    printMenu();
    menuChoice = getMenuChoice();
    switch (menuChoice) {
        case 6:
            randomMode = true;
            Console.WriteLine("You will be presented with 10 random questions. Good luck!");
            for (int i = 0; i < 10; i++) {
                score = await createPuzzle(menuChoice, history, currDiff, score, randomMode);
            }
            break;
        case 7:
            Console.WriteLine("Game History: ");
            foreach (var record in history) {
                Console.WriteLine($"{record}");
            }
            break;
        case 8:
            currDiff = ChangeDifficulty();
            Console.WriteLine($"Current difficulty level: {currDiff}");
            break;
        case 9:
            Console.WriteLine($"Your total score is {score}!");
            Console.WriteLine($"The total of {history.Count} games played!");
            gameOn = false;
            break;
        default:
            score = await createPuzzle(menuChoice, history, currDiff, score, randomMode);
            break;
    }
    if (menuChoice > 0 && menuChoice < 8){
        Console.WriteLine($"Your current score is {score}!");
    }
}

static void printMenu() {
    Console.WriteLine("Please enter your choice (1-9): ");
    Console.WriteLine("1. Addition");
    Console.WriteLine("2. Subtraction");
    Console.WriteLine("3. Multiplication");
    Console.WriteLine("4. Division");
    Console.WriteLine("5. Square of a number");
    Console.WriteLine("6. Random mode");
    Console.WriteLine("7. View previous games");
    Console.WriteLine("8. Change difficulty");
    Console.WriteLine("9. Exit");
}

static int getMenuChoice() {
    int menuChoice = -1;
    while (!int.TryParse(Console.ReadLine(), out menuChoice) || menuChoice < 1 || menuChoice > 9) {
        Console.WriteLine("Please enter a valid choice (1-9): ");
    }
    return menuChoice;
}

static async Task<int> createPuzzle(int op, List<string> history, DifficultyLevel currDiff, int score, bool randomMode) {
    Random rand = new Random();
    int x = rand.Next(1, 101);
    int y = rand.Next(1, 101);
    int trueRes = 0;
    int? resp = -1;
    if (randomMode) {
        op = rand.Next(1, 6);
    }
    switch (op) {
        case 1:
            history.Add($"{x} + {y} = {x+y}");
            Console.WriteLine($"{x} + {y} = ");
            trueRes = x + y;
            break;
        case 2:
            history.Add($"{x} - {y} = {x-y}");
            Console.WriteLine($"{x} - {y} = ");
            trueRes = x - y;
            break;
        case 3:
            y = rand.Next(1, 12);
            history.Add($"{x} * {y} = {x*y}");
            Console.WriteLine($"{x} * {y} = ");
            trueRes = x * y;
            break;
        case 4:
            y = rand.Next(1, 12);
            x = x*y;
            y = x/y;
            history.Add($"{x} / {y} = {x/y}");
            Console.WriteLine($"{x} / {y} = ");
            trueRes = x / y;
            break;
        case 5:
            history.Add($"{x} * {x} = {x*x}");
            Console.WriteLine($"{x} * {x} = ");
            trueRes = x * x;
            break;
    }
    resp = await getUserResp(currDiff);
    if (resp == trueRes) {
        Console.WriteLine("Correct!");
        score += 10;
    }
    else {
        Console.WriteLine($"A pity! The correct answer was {trueRes}");
    }
    return score;
}

static async Task<int?> getUserResp(DifficultyLevel diff) {
    int resp = 0;
    int timeout = (int) diff;
    string? result;
    CancellationTokenSource cts = new CancellationTokenSource();

    Console.WriteLine("Enter your answer: ");
    Stopwatch watch = new Stopwatch();
    watch.Start();

    Task<string?> getResp = Task.Run(() => {
        try {
            while (!cts.Token.IsCancellationRequested) {
                if (Console.KeyAvailable) {
                    string? ans = Console.ReadLine();
                    return ans;
                }
                cts.Token.ThrowIfCancellationRequested();
            }
        }
        catch (OperationCanceledException) {
            return null;
        }
        return null;
    }, cts.Token);

    try {      
        if (await Task.WhenAny(getResp, Task.Delay(timeout * 1000)) == getResp) {
            result = getResp.Result;
        }
        else {
            result = null;
            cts.Cancel();
        }
        watch.Stop();
        if (result == null) {
            throw new TimeoutException();
        }
        else if (!int.TryParse(result, out resp)) {
            throw new ArithmeticException();
        }
        else {
            Console.WriteLine($"Time taken to answer: {watch.Elapsed.ToString(@"m\:ss\.fff")}");
            return resp;
        }
    }
    catch (TimeoutException) {
        Console.WriteLine("Sorry, you ran out of time!");
        return null;
    }
    catch (ArithmeticException) {
        Console.WriteLine("The answer should be numerical!");
        return null;
    }
}

static DifficultyLevel ChangeDifficulty() {
    Console.WriteLine("Please enter the difficulty level (1-3): ");
    Console.WriteLine("1. Easy");
    Console.WriteLine("2. Medium");
    Console.WriteLine("3. Hard");
    int diffChoice = 0;
    while (!int.TryParse(Console.ReadLine(), out diffChoice) || diffChoice < 1 || diffChoice > 3) {
        Console.WriteLine("Please enter a valid choice (1-3): ");
    }
    return diffChoice switch {
        1 => DifficultyLevel.Easy,
        2 => DifficultyLevel.Medium,
        3 => DifficultyLevel.Hard
    };
}

public enum DifficultyLevel {
        Easy = 40,
        Medium = 30,
        Hard = 20
}
