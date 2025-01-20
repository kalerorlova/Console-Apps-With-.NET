using System;

public interface ICodingSessionRepository {
    public Task createSession(string startTime, string endTime, string duration);
    public Task<List<CodingSession>> getAllSessions();
    public Task<List<CodingSession>> checkID(int id);
    public Task updateSession(int id, string startTime, string endTime, string duration);
    public Task deleteSession(int id);
}