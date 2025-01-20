using System;
using System.Collections.Generic;
using System.Globalization;
using Spectre.Console;

public static class Input {
    public const string dateFormat = "MM/dd/yyyy HH:mm";

    public static DateTime getDate(string dateType, string promptType, DateTime? startAt = null) {
        string strDate;
        DateTime date;
        do {
            strDate = AnsiConsole.Prompt(new TextPrompt<string>(
                $"Please enter the {dateType} date of the {promptType} in the following (24hr clock) format: " + dateFormat));
            }
        while (!DateTime.TryParseExact(strDate, dateFormat, new CultureInfo("en-US"), DateTimeStyles.None, out date));
        AnsiConsole.WriteLine($"Date entered is {date}");
        bool confirmed = AnsiConsole.Prompt(new ConfirmationPrompt("Is the entered date correct? Please pay attention to the format"));
        if (!confirmed) {
            date = getDate(dateType, promptType);
        }
        if (dateType == "end") {
            if (date <= startAt) {
                AnsiConsole.WriteLine("This simply cannot be! Re-enter the end date");
                getDate(dateType, promptType, startAt);
            }
        }
        return date;
    }

    public static void printTable(List<CodingSession> sessions) {
        var table = new Table();
        table.AddColumns("ID", "Start Time", "End Time", "Session duration (in hours)");
        table.Centered();
            foreach (var session in sessions) {
                table.AddRow(session.id.ToString(), session.startTime, session.endTime, session.duration);
            }
        AnsiConsole.Write(table);
    }

}