
using Dapper;
using Microsoft.Data.SqlClient;

internal static class sessionRepo {

    internal static void logSession(string name, DateTime date, int score) {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            connection.Execute(@"INSERT INTO Sessions (stackName, sessionDate, sessionScore) VALUES
                        (@stackName, @sessionDate, @sessionScore)", new {stackName = name, sessionDate = date, sessionScore = score});
        }
    }

    internal static List<Session> getAll() {
        using (var connection = new SqlConnection(AppConfig.databaseConnection)) {
            connection.Open();
            List<Session> myList = connection.Query<Session>("SELECT * FROM Sessions").ToList();
            return myList;
        }
    }
        
}
