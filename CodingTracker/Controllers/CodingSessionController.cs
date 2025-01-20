using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.VisualBasic;
using Spectre.Console;

internal class CodingSessionController(ICodingSessionRepository cdRepo) {

        internal async void createSession() {
                Console.Clear();
                DateTime startDate = Input.getDate("start");
                DateTime endDate = Input.getDate("end", startDate);
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

        internal async void readSessions() {
                Console.Clear();
                var sessions = await cdRepo.getAllSessions();
                var table = new Table();
                table.AddColumns("ID", "Start Time", "End Time", "Session duration (in hours)");
                table.Centered();
                foreach (var session in sessions) {
                        table.AddRow(session.id.ToString(), session.startTime, session.endTime, session.duration);
                }
                AnsiConsole.Write(table);
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
                        updateStart = Input.getDate("start");
                }
                confirmed = AnsiConsole.Prompt(new ConfirmationPrompt("Would you like to update the end date?"));
                if (confirmed) {
                        updateStart = Input.getDate("end", updateStart);
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
}
