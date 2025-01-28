
using System.Data.Common;
using Dapper;
using Microsoft.Data.SqlClient;
using Spectre.Console;

internal static class flashcardRepo {

    internal static void create(string stackName, string question, string answer) {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            connection.Execute(@"INSERT INTO Flashcards (stackName, question, answer) VALUES 
                            (@StackName, @Question, @Answer)", new {StackName = stackName, Question = question, Answer = answer});
        }
    }

    internal static void updateInfo(string infoType, string info, int? id) {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            connection.Execute($"UPDATE Flashcards SET {infoType} = @Info WHERE flashcardID = @ID", new {Info = info, ID = id});
        }
        Console.Clear();
        AnsiConsole.WriteLine($"Flashcard {infoType} updated successfully!");
    }

    internal static void updateInfo(int? stackID, int? id) {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            string? stackName = connection.ExecuteScalar<string>(@"SELECT stackName FROM Stacks WHERE stackID = @ID", new {ID = stackID});
            connection.Execute($"UPDATE Flashcards SET stackName = @name WHERE flashcardID = @ID", new {name = stackName, ID = id});
            Console.Clear();
            AnsiConsole.WriteLine($"Flashcard moved to the stack {stackName} successfully!");
        }
    }

    internal static void delete (int? id) {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            connection.Execute("DELETE FROM Flashcards WHERE flashcardID = @ID", new {ID = id});
        }
    }

    internal static Flashcard? getCard (int id) {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            Flashcard? card = connection.QuerySingleOrDefault<Flashcard>(@"SELECT * FROM Flashcards WHERE flashcardID = @ID", new {ID = id});
            return card;
        }
    }

    internal static int? checkExistence(int id) {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            int? res = connection.ExecuteScalar<int?>(@"SELECT flashcardID FROM Flashcards WHERE flashcardID = @ID", new {ID = id});
            return res;
        }
    }
        

}