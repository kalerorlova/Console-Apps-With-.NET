using System;
using Microsoft.Data.Sqlite;

using (var connection = new SqliteConnection("Data Source=C:/SQLite/habits.db")) {
    connection.Open();
    var command = connection.CreateCommand();
    command.CommandText = @"CREATE TABLE IF NOT EXISTS Habits 
                (entryID INTEGER PRIMARY KEY AUTOINCREMENT,
                habitName TEXT UNIQUE NOT NULL,
                date TEXT NOT NULL,
                quantity INTEGER NOT NULL)";
    command.ExecuteNonQuery();
    connection.Close();
}
accessDb();

static void accessDb() {
    int menuChoice;
    bool appOpen = true;
    while (appOpen) {
        Console.WriteLine("Choose the operation you want to perform (1-4): ");
        Console.WriteLine("1. Create a new record");
        Console.WriteLine("2. Read/get all records");
        Console.WriteLine("3. Update a record");
        Console.WriteLine("4. Delete a record");
        Console.WriteLine("5. Exit the app");
        while (!int.TryParse(Console.ReadLine(), out menuChoice) || menuChoice < 0 || menuChoice > 5) {
            Console.WriteLine("Please enter a valid choice (1-4): ");
        }
        switch (menuChoice) {
            case 1:
                createRecord();
                break;
            case 2:
                getRecords();
                break;
            case 3:
                updateRecord();
                break;
            case 4:
                deleteRecord();
                break;
            case 5:
                appOpen = false;
                Console.WriteLine("Thank you for using our database!\nClosing the app..");
                break;
        }
    }
}

static void createRecord() {
    Console.WriteLine("Please enter the habit name: ");
    string habit = Console.ReadLine();
    int quantity;
    Console.WriteLine("Please enter the quantity: ");
    while (!int.TryParse(Console.ReadLine(), out quantity) || quantity < 1) {
        Console.WriteLine("Please enter a valid quantity (> 0): ");
    }
    using (var connection = new SqliteConnection("Data Source=C:/SQLite/habits.db")) {
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Habits
            (habitName, date, quantity) VALUES
            ($habitName, $date, $quantity)";
        command.Parameters.AddWithValue("$habitName", habit);
        command.Parameters.AddWithValue("$date", DateTime.Now.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$quantity", quantity);
        command.ExecuteNonQuery();
        connection.Close();
    }
}

static void getRecords() {
    Console.WriteLine("Here is the list of all logged habits: ");
    using (var connection = new SqliteConnection("Data Source=C:/SQLite/habits.db")) {
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM Habits";
        using (var reader = command.ExecuteReader()) {
            while (reader.Read()) {
                Console.WriteLine($"{reader.GetString(1)}: {reader.GetInt32(3)} time(s), on {reader.GetString(2)}");
            }
            reader.Close();
        }
        connection.Close();
    }
}

static void updateRecord() {
    int id = retrieveRecord();
    if (id == -1) {
        Console.WriteLine("The entry with given parameters does not exist!");
        return;
    }
    int newNum;
    Console.WriteLine("Please enter the new number of occurences of the habit: ");
    while (!int.TryParse(Console.ReadLine(), out newNum) || newNum < 1) {
        Console.WriteLine("Please re-enter the number (>0): ");
    }
    using (var connection = new SqliteConnection("Data Source=C:/SQLite/habits.db")) {
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"UPDATE Habits SET quantity = $newNum WHERE entryID = $id";
        command.Parameters.AddWithValue("$newNum", newNum);
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
        connection.Close();
    }
}

static void deleteRecord() {
    int id = retrieveRecord();
    if (id == -1) {
        Console.WriteLine("The entry with given parameters does not exist!");
        return;
    }
    using (var connection = new SqliteConnection("Data Source=C:/SQLite/habits.db")) {
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM Habits WHERE entryID = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
        connection.Close();
    }
}

static int retrieveRecord() {
    Console.WriteLine("Do you know the ID of the record? (y/n)");
    string ans = Console.ReadLine();
    while (ans != "y" && ans != "n") {
        Console.WriteLine("Please type your answer again (y/n)");
        ans = Console.ReadLine();
    }
    int id;
    switch (ans) {
        case "y":
            Console.WriteLine("Please type the ID of the record you're interested in: ");
            while (!int.TryParse(Console.ReadLine(), out id)) {
                Console.WriteLine("ID should be a number. Please type a number: ");
            }
            using (var connection = new SqliteConnection("Data Source=C:/SQLite/habits.db")) {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Habits WHERE entryID = $id";
                command.Parameters.AddWithValue("$id", id);
                object result = command.ExecuteScalar();
                connection.Close();
                id = (result == null) ? -1 : id;
                return id;
            }
        case "n":
            Console.WriteLine("Please type the name of the habit you're interested in: ");
            string habit = Console.ReadLine();
            Console.WriteLine("Please type the date of the occurence (YYYY-MM-DD)");
            string date = Console.ReadLine();
            using (var connection = new SqliteConnection("Data Source=C:/SQLite/habits.db")) {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT entryID FROM Habits WHERE habitName = $habit AND date = $date";
                command.Parameters.AddWithValue("$habit", habit);
                command.Parameters.AddWithValue("$date", date);
                object result = command.ExecuteScalar();
                connection.Close();
                if (result == null) {
                    id = -1;
                }
                else {
                    int.TryParse(result.ToString(), out id);
                }
                return id;
            }
    }
    return -1;
}