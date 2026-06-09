using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;

namespace _2.semEksamenProjekt.Services
{
    public class Database
    {
        private const string ConnectionString = "Data Source=users.db";

        public static void Initialize()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Username TEXT NOT NULL UNIQUE,
            Navn TEXT NOT NULL,
            Password TEXT NOT NULL,
            RolleId INTEGER,
            FOREIGN KEY (RolleId) REFERENCES Roller(Id)
        );
        CREATE TABLE IF NOT EXISTS Roller (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Navn TEXT NOT NULL UNIQUE
        );
        CREATE TABLE IF NOT EXISTS Flows (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Navn TEXT NOT NULL UNIQUE,
            Beskrivelse TEXT
        );
        CREATE TABLE IF NOT EXISTS UserFlows (
            UserId INTEGER,
            FlowId INTEGER,
            PRIMARY KEY (UserId, FlowId),
            FOREIGN KEY (UserId) REFERENCES Users(Id),
            FOREIGN KEY (FlowId) REFERENCES Flows(Id)
        );

        CREATE TABLE IF NOT EXISTS Lektioner (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Titel TEXT NOT NULL,
            Indhold TEXT,
            Dag TEXT NOT NULL,
            StartTid TEXT NOT NULL,
            SlutTid TEXT NOT NULL,
            FlowId INTEGER,
            FOREIGN KEY (FlowId) REFERENCES Flows(Id)
        );

        CREATE TABLE IF NOT EXISTS FlowIndhold (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FlowId INTEGER NOT NULL,
            Titel TEXT NOT NULL,
            Tekst TEXT,
            Sortering INTEGER DEFAULT 0,
            FOREIGN KEY (FlowId) REFERENCES Flows(Id)
        );

        CREATE TABLE IF NOT EXISTS Dokumenter (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FlowIndholdId INTEGER NOT NULL,
            Filnavn TEXT NOT NULL,
            Filsti TEXT NOT NULL,
            FOREIGN KEY (FlowIndholdId) REFERENCES FlowIndhold(Id)
        );";
            command.ExecuteNonQuery();

            var alterNavn = connection.CreateCommand();
            alterNavn.CommandText = "ALTER TABLE Users ADD COLUMN Navn TEXT;";
            try { alterNavn.ExecuteNonQuery(); } catch { }

            var alterBeskrivelse = connection.CreateCommand();
            alterBeskrivelse.CommandText = "ALTER TABLE Flows ADD COLUMN Beskrivelse TEXT;";
            try { alterBeskrivelse.ExecuteNonQuery(); } catch { }

        }

        public static bool ValidateUser(string username, string password)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            var result = (long)command.ExecuteScalar();
            return result > 0;
        }

        public static void SeedDefaultUser()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
        INSERT OR IGNORE INTO Roller (Navn) VALUES ('Administrativ');
        INSERT OR IGNORE INTO Roller (Navn) VALUES ('Lærer');
        INSERT OR IGNORE INTO Roller (Navn) VALUES ('Elev');
        INSERT OR IGNORE INTO Users (Navn, Username, Password, RolleId) 
        VALUES ('Administrator', 'admin', '1234', 1);";
            command.ExecuteNonQuery();
        }

        public static List<string> GetAllUsers()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT u.Username, r.Navn 
                FROM Users u 
                LEFT JOIN Roller r ON u.RolleId = r.Id";

            var users = new List<string>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add($"{reader.GetString(0)} ({reader.GetString(1)})");
            }
            return users;
        }

        public static void CreateUser(string navn, string username, string password, string rolle)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR IGNORE INTO Users (Navn, Username, Password, RolleId)
                VALUES (@navn, @u, @p, (SELECT Id FROM Roller WHERE Navn = @rolle))";
            command.Parameters.AddWithValue("@navn", navn);
            command.Parameters.AddWithValue("@u", username);
            command.Parameters.AddWithValue("@p", password);
            command.Parameters.AddWithValue("@rolle", rolle);
            command.ExecuteNonQuery();
        }

        public static void DeleteUser(string username)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE Username = @u";
            command.Parameters.AddWithValue("@u", username);
            command.ExecuteNonQuery();
        }

        public static void UpdateUser(string originalUsername, string nytNavn, string nytUsername, string nytPassword, string nyRolle)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
        UPDATE Users SET Navn = @navn, Username = @nytU, Password = @nytP,
        RolleId = (SELECT Id FROM Roller WHERE Navn = @rolle)
        WHERE Username = @origU";
            command.Parameters.AddWithValue("@navn", nytNavn);
            command.Parameters.AddWithValue("@nytU", nytUsername);
            command.Parameters.AddWithValue("@nytP", nytPassword);
            command.Parameters.AddWithValue("@rolle", nyRolle);
            command.Parameters.AddWithValue("@origU", originalUsername);
            command.ExecuteNonQuery();
        }

        public static List<string> GetAllRoller()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Navn FROM Roller";

            var roller = new List<string>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                roller.Add(reader.GetString(0));
            }
            return roller;
        }

        public static List<string> GetAllFlows()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Navn FROM Flows";

            var flows = new List<string>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                flows.Add(reader.GetString(0));
            }
            return flows;
        }

        public static void CreateFlow(string navn, string beskrivelse, string username)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT OR IGNORE INTO Flows (Navn, Beskrivelse) VALUES (@navn, @beskrivelse)";
            cmd.Parameters.AddWithValue("@navn", navn);
            cmd.Parameters.AddWithValue("@beskrivelse", beskrivelse);
            cmd.ExecuteNonQuery();

            var idCmd = connection.CreateCommand();
            idCmd.CommandText = "SELECT Id FROM Flows WHERE Navn = @navn";
            idCmd.Parameters.AddWithValue("@navn", navn);
            int flowId = Convert.ToInt32(idCmd.ExecuteScalar());

            var userCmd = connection.CreateCommand();
            userCmd.CommandText = "SELECT Id FROM Users WHERE Username = @username";
            userCmd.Parameters.AddWithValue("@username", username);
            int userId = Convert.ToInt32(userCmd.ExecuteScalar());

            var linkCmd = connection.CreateCommand();
            linkCmd.CommandText = "INSERT OR IGNORE INTO UserFlows (UserId, FlowId) VALUES (@userId, @flowId)";
            linkCmd.Parameters.AddWithValue("@userId", userId);
            linkCmd.Parameters.AddWithValue("@flowId", flowId);
            linkCmd.ExecuteNonQuery();
        }

        public static void DeleteFlow(string navn)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Flows WHERE Navn = @navn";
            command.Parameters.AddWithValue("@navn", navn);
            command.ExecuteNonQuery();
        }

        public static (string navn, string username, string password, string rolle) GetUser(string username)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT u.Navn, u.Username, u.Password, r.Navn 
        FROM Users u 
        LEFT JOIN Roller r ON u.RolleId = r.Id
        WHERE u.Username = @username";
            command.Parameters.AddWithValue("@username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return (reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
            }
            return ("", username, "", "");
        }

        public static (string navn, string beskrivelse) GetFlow(string navn)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Navn, Beskrivelse FROM Flows WHERE Navn = @navn";
            command.Parameters.AddWithValue("@navn", navn);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return (reader.GetString(0), reader.IsDBNull(1) ? "" : reader.GetString(1));
            }
            return (navn, "");
        }

        public static void UpdateFlow(string originalNavn, string nytNavn, string nyBeskrivelse)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
        UPDATE Flows SET Navn = @nytNavn, Beskrivelse = @beskrivelse 
        WHERE Navn = @originalNavn";
            command.Parameters.AddWithValue("@nytNavn", nytNavn);
            command.Parameters.AddWithValue("@beskrivelse", nyBeskrivelse);
            command.Parameters.AddWithValue("@originalNavn", originalNavn);
            command.ExecuteNonQuery();
        }

        public static void TilmeldFlow(string username, string flowNavn)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
        INSERT OR IGNORE INTO UserFlows (UserId, FlowId)
        VALUES (
            (SELECT Id FROM Users WHERE Username = @username),
            (SELECT Id FROM Flows WHERE Navn = @flowNavn)
        )";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@flowNavn", flowNavn);
            command.ExecuteNonQuery();
        }

        public static void AfmeldFlow(string username, string flowNavn)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
        DELETE FROM UserFlows 
        WHERE UserId = (SELECT Id FROM Users WHERE Username = @username)
        AND FlowId = (SELECT Id FROM Flows WHERE Navn = @flowNavn)";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@flowNavn", flowNavn);
            command.ExecuteNonQuery();
        }

        public static List<string> GetFlowsForUser(string username)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT f.Navn FROM Flows f
        JOIN UserFlows uf ON f.Id = uf.FlowId
        JOIN Users u ON u.Id = uf.UserId
        WHERE u.Username = @username";
            command.Parameters.AddWithValue("@username", username);

            var flows = new List<string>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                flows.Add(reader.GetString(0));
            }
            return flows;
        }

        public static List<string> GetUsersForFlow(string flowNavn)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT u.Navn, u.Username FROM Users u
        JOIN UserFlows uf ON u.Id = uf.UserId
        JOIN Flows f ON f.Id = uf.FlowId
        WHERE f.Navn = @flowNavn";
            command.Parameters.AddWithValue("@flowNavn", flowNavn);

            var users = new List<string>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add($"{reader.GetString(0)} ({reader.GetString(1)})");
            }
            return users;
        }
        public class Bruger
        {
            public int Id { get; set; }
            public string Navn { get; set; }
            public string Username { get; set; }

            public override string ToString()
            {
                return $"{Navn} ({Username})";
            }
        }

        public static List<Bruger> GetAllBrugere()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Navn, Username FROM Users";

            var brugere = new List<Bruger>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                brugere.Add(new Bruger
                {
                    Id = reader.GetInt32(0),
                    Navn = reader.GetString(1),
                    Username = reader.GetString(2)
                });
            }
            return brugere;
        }
        public static string GetUserRole(string username, string password)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT r.Navn FROM Users u
        JOIN Roller r ON u.RolleId = r.Id
        WHERE u.Username = @username AND u.Password = @password";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            var result = command.ExecuteScalar();
            return result?.ToString();
        }

        public static void OpretLektion(string titel, string indhold, string dag, string startTid, string slutTid, string flowNavn)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
        INSERT INTO Lektioner (Titel, Indhold, Dag, StartTid, SlutTid, FlowId)
        VALUES (@titel, @indhold, @dag, @startTid, @slutTid,
            (SELECT Id FROM Flows WHERE Navn = @flowNavn))";
            command.Parameters.AddWithValue("@titel", titel);
            command.Parameters.AddWithValue("@indhold", indhold);
            command.Parameters.AddWithValue("@dag", dag);
            command.Parameters.AddWithValue("@startTid", startTid);
            command.Parameters.AddWithValue("@slutTid", slutTid);
            command.Parameters.AddWithValue("@flowNavn", flowNavn);
            command.ExecuteNonQuery();
        }

        public static List<(string titel, string dag, string startTid, string slutTid, string flowNavn)> GetLektionerForUser(string username)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT l.Titel, l.Dag, l.StartTid, l.SlutTid, f.Navn
        FROM Lektioner l
        JOIN Flows f ON f.Id = l.FlowId
        JOIN UserFlows uf ON uf.FlowId = f.Id
        JOIN Users u ON u.Id = uf.UserId
        WHERE u.Username = @username";
            command.Parameters.AddWithValue("@username", username);

            var lektioner = new List<(string, string, string, string, string)>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lektioner.Add((
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4)
                ));
            }
            return lektioner;
        }

        public static void OpretUnderflow(string titel, string tekst, int flowId)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
        INSERT INTO FlowIndhold (FlowId, Titel, Tekst, Sortering)
        VALUES (@flowId, @titel, @tekst,
            (SELECT COALESCE(MAX(Sortering), 0) + 1 FROM FlowIndhold WHERE FlowId = @flowId))";
            command.Parameters.AddWithValue("@flowId", flowId);
            command.Parameters.AddWithValue("@titel", titel);
            command.Parameters.AddWithValue("@tekst", tekst);
            command.ExecuteNonQuery();
        }

        public static List<(int id, string titel, string tekst)> GetUnderflows(string flowNavn)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT fi.Id, fi.Titel, fi.Tekst 
        FROM FlowIndhold fi
        JOIN Flows f ON f.Id = fi.FlowId
        WHERE f.Navn = @flowNavn
        ORDER BY fi.Sortering";
            command.Parameters.AddWithValue("@flowNavn", flowNavn);

            var liste = new List<(int, string, string)>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                liste.Add((reader.GetInt32(0), reader.GetString(1), reader.IsDBNull(2) ? "" : reader.GetString(2)));
            }
            return liste;
        }

        public static int GetFlowId(string flowNavn)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id FROM Flows WHERE Navn = @navn";
            command.Parameters.AddWithValue("@navn", flowNavn);
            var result = command.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : -1;
        }
    }
}
