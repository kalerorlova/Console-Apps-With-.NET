using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.VisualBasic;
using Spectre.Console;

internal class CodingSessionController(ICodingSessionRepository cdRepo) {

        internal async void createSession() {
                Console.Clear();
                DateTime startDate = Input.getDate("start", "session");
                DateTime endDate = Input.getDate("end", "session", startDate);
                TimeSpan duration = endDate - startDate;
                if (duration.Days > 0) {
                        AnsiConsole.WriteLine("Not adding your session, cheater");
                        return;
                }
                await cdRepo.createSession(startDate.ToString(Input.dateFormat), endDate.ToString(Input.dateFormat)
                        , duration.ToString(@"hh\:mm"));
                Console.Clear();
                AnsiConsole.WriteLine("Session created successfully!");              
        }

        internal async void createTimedSession() {
                Console.Clear();
                DateTime startDate;
                DateTime endDate;
                TimeSpan duration;
                var stopwatch = new Stopwatch();
                bool stopwatchOn = true;
                string menuChoice = "Resume";
                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                        .Title("Ready to start the session? ")
                                        .AddChoices("Return to the menu", "Start the session"));
                if (choice == "Return to menu") {
                        return;
                }
                startDate = DateTime.Now;
                while (stopwatchOn && menuChoice != "Finish") {
                        switch (menuChoice) {
                                case "Resume":
                                        stopwatch.Start();
                                        stopwatchOn = true;
                                        menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                                .Title("Would you like to pause or finish the session? ")
                                                .AddChoices("Pause", "Finish"));
                                        break;
                                case "Pause":
                                        stopwatch.Stop();
                                        stopwatchOn = true;
                                        menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                                .Title("Would you like to resume the session? ")
                                                .AddChoices("Pause", "Resume"));
                                        break;
                                case "Finish":
                                        stopwatch.Stop();
                                        menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                                .Title("Would you like to finish the session? ")
                                                .AddChoices("Resume", "Finish"));
                                        stopwatchOn = false;
                                        break;          
                        }
                }
                endDate = DateTime.Now;
                duration = stopwatch.Elapsed;
                await cdRepo.createSession(startDate.ToString(Input.dateFormat), endDate.ToString(Input.dateFormat)
                        , duration.ToString(@"hh\:mm"));
                Console.Clear();
                AnsiConsole.WriteLine("Session created successfully!");
        }

        internal async void readSessions() {
                Console.Clear();
                var sessions = await cdRepo.getAllSessions();
                Input.printTable(sessions);
        }

        internal async void updateSession() {
                Console.Clear();
                int id = AnsiConsole.Prompt(new TextPrompt<int>("Please type the ID of the record you're interested in: "));
                var session = await cdRepo.checkID(id);
                if (session.Count == 0) {
                        AnsiConsole.WriteLine("The entry with given id does not exist!");
                        return;
                }
                DateTime initStart;
                DateTime initEnd;
                DateTime.TryParseExact(session[0].startTime, Input.dateFormat, new CultureInfo("en-US"), DateTimeStyles.None, out initStart);
                DateTime.TryParseExact(session[0].endTime, Input.dateFormat, new CultureInfo("en-US"), DateTimeStyles.None, out initEnd);
                DateTime updateStart = initStart;
                DateTime updateEnd = initEnd;
                bool confirmed = AnsiConsole.Prompt(new ConfirmationPrompt("Would you like to update the start date?"));
                if (confirmed) {
                        updateStart = Input.getDate("start", "session");
                }
                confirmed = AnsiConsole.Prompt(new ConfirmationPrompt("Would you like to update the end date?"));
                if (confirmed) {
                        updateStart = Input.getDate("end", "session", updateStart);
                }
                TimeSpan duration = updateEnd - updateStart;
                if (duration.Days != 0 ) {
                        AnsiConsole.WriteLine("Not adding your session, cheater");
                        return;
                }
                await cdRepo.updateSession(id, updateStart.ToString(Input.dateFormat), updateEnd.ToString(Input.dateFormat)
                        , duration.ToString(@"hh\:mm"));
                Console.Clear();
                AnsiConsole.WriteLine("Session updated successfully!");     
        }

        internal async void deleteSession() {
                Console.Clear();
                int id = AnsiConsole.Prompt(new TextPrompt<int>("Please type the ID of the record you're interested in: "));
                var session = await cdRepo.checkID(id);
                if (session.Count == 0) {
                        AnsiConsole.WriteLine("The entry with the given id does not exist!");
                        return;
                }
                await cdRepo.deleteSession(id);
                AnsiConsole.WriteLine("Session deleted successfully!");
        }

        internal async void filterSessions() {
                Console.Clear();
                DateTime startDate = Input.getDate("start", "period");
                DateTime endDate = Input.getDate("start", "period");
                var sessions = await cdRepo.getSessionsByPeriod(startDate.ToString(Input.dateFormat), endDate.ToString(Input.dateFormat));
                Input.printTable(sessions);
        }

        internal async void runSummary() {
                Console.Clear();
                DateTime startDate = Input.getDate("start", "period");
                DateTime endDate = Input.getDate("start", "period");
                var times = await cdRepo.runSummary(startDate.ToString(Input.dateFormat), endDate.ToString(Input.dateFormat));
                if (times.Count == 0) {
                        AnsiConsole.WriteLine("No sessions entered yet!");
                        return;
                }
                if (times.Count == 1) {
                        AnsiConsole.WriteLine($"You have been coding for {times[0]} hours total during that period");
                        return;
                }
                TimeSpan sum = new TimeSpan(0, 0, 0);
                foreach (var time in times) {
                        sum += TimeSpan.Parse(time);
                }
                AnsiConsole.WriteLine($"You have been coding for {sum.ToString(@"hh\:mm")} hrs:mins total during that period!");
                return;
        }
}
