using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2.semEksamenProjekt.Services
{
    public class FlowDbService
    {
        private const string ConnectionString = "Data Source=users.db";

        public static List<string> GetAllFlows()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Navn FROM Flows";
            var flows = new List<string>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
                flows.Add(reader.GetString(0));
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

        public static (string navn, string beskrivelse) GetFlow(string navn)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Navn, Beskrivelse FROM Flows WHERE Navn = @navn";
            command.Parameters.AddWithValue("@navn", navn);
            using var reader = command.ExecuteReader();
            if (reader.Read())
                return (reader.GetString(0), reader.IsDBNull(1) ? "" : reader.GetString(1));
            return (navn, "");
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
                flows.Add(reader.GetString(0));
            return flows;
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
                users.Add($"{reader.GetString(0)} ({reader.GetString(1)})");
            return users;
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
                liste.Add((reader.GetInt32(0), reader.GetString(1), reader.IsDBNull(2) ? "" : reader.GetString(2)));
            return liste;
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
    }
}