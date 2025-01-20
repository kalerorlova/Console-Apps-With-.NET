using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
/*
internal interface ICodingSessionRepository : IDisposable {
    internal Task createSession(DateTime startTime, DateTime endTime);
    internal Task<List<CodingSession>?> getAllSessions();
    internal Task updateSession(DateTime? startTime, DateTime? endTime, int? id);
    internal Task deleteSession(int? id);
}
*/

public class CodingSessionRepository : ICodingSessionRepository {
    public async Task createSession(string StartTime, string EndTime, string Duration) {
        using (var connection = new SqliteConnection(AppConfig.connectionString)) {
            string sql = "INSERT INTO Code_Sessions (startTime, endTime, duration) " +
            "VALUES (@startTime, @endTime, @duration)";
            await connection.ExecuteAsync(sql, new {startTime = StartTime, endTime = EndTime, duration = Duration});
        }
    }

    public async Task<List<CodingSession>> getAllSessions() {
        using (var connection = new SqliteConnection(AppConfig.connectionString)) {
            string sql = "SELECT * FROM Code_Sessions";
            var sessions = (await connection.QueryAsync<CodingSession>(sql)).ToList();
            return sessions;
        }
    }

    public async Task<List<CodingSession>> checkID(int ID) {
        using (var connection = new SqliteConnection(AppConfig.connectionString)) {
            string sql = "SELECT * FROM Code_Sessions WHERE id = @id";
            var session = (await connection.QueryAsync<CodingSession>(sql, new {id = ID})).ToList();
            return session;
        }
    }

    public async Task updateSession(int id, string start, string end, string duration) {
        using (var connection = new SqliteConnection(AppConfig.connectionString)) {
            string sql = "UPDATE Code_Sessions SET startTime = @startQ, endTime = @endQ, duration = @durationQ WHERE id = @idQ";
            await connection.ExecuteAsync(sql, new {idQ = id, startQ = start, endQ = end, durationQ = duration});
        }
    }

    public async Task deleteSession(int ID) {
        using (var connection = new SqliteConnection(AppConfig.connectionString)) {
            string sql = "DELETE FROM Code_Sessions WHERE id = @id";
            await connection.ExecuteAsync(sql, new {id = ID});
        }
    }


}



