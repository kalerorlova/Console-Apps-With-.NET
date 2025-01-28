
using Dapper;
using Microsoft.Data.SqlClient;
using Spectre.Console;

internal static class stackRepo {

    internal static void createStack(string name) {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            connection.Execute(@"INSERT INTO Stacks VALUES (@Name)", new {Name = name});
        }
    }

    internal static void updateName(string name, int? origId) {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            connection.Execute(@"UPDATE Stacks SET stackName = @Name WHERE stackID = @ID", new {Name = name, ID = origId});
        }
    }

    internal static void delete(string name) {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            connection.Execute(@"DELETE FROM Stacks WHERE stackname = @Name", new {Name = name});
        }
    }

    internal static List<Flashcard> getStack(string name) {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            List<Flashcard> myList = connection.Query<Flashcard>(
                "SELECT * FROM Flashcards WHERE stackName = @Name", new {Name = name}).ToList();
            return myList;
        }
    } 

    internal static List<string> getAll() {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            List<string> names = connection.Query<string>("SELECT stackName FROM Stacks").ToList();
            return names;
        }
    }

    internal static int? checkExistence(string name) {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            int? id = connection.ExecuteScalar<int?>(@"SELECT stackID FROM Stacks WHERE stackName = @Name", new {Name = name});
            return id;
        }
    }

}
