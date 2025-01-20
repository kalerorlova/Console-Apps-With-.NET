using System;
using System.Collections.Generic;
using System.Globalization;
using Spectre.Console;

public static class Input {
    public const string dateFormat = "MM/dd/yyyy HH:mm";


    public static DateTime getDate(string dateType, DateTime? startAt = null) {
        string strDate;
        DateTime date;
        do {
            strDate = AnsiConsole.Prompt(new TextPrompt<string>(
                $"Please enter the {dateType} date of the session in the following (24hr clock) format: " + dateFormat));
            }
        while (!DateTime.TryParseExact(strDate, dateFormat, new CultureInfo("en-US"), DateTimeStyles.None, out date));
        AnsiConsole.WriteLine($"Date entered is {date}");
        bool confirmed = AnsiConsole.Prompt(new ConfirmationPrompt("Is the entered date correct? Please pay attention to the format"));
        if (!confirmed) {
            date = getDate(dateType);
        }
        if (dateType == "end") {
            if (date <= startAt) {
                AnsiConsole.WriteLine("This simply cannot be! Re-enter the end date");
                getDate(dateType, startAt);
            }
        }
        return date;
    }

}