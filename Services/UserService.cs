using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2.semEksamenProjekt.Services
{
    public class UserService
    {
        private const string ConnectionString = "Data Source=users.db";

        public static bool ValidateUser(string username, string password)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);
            return (long)command.ExecuteScalar() > 0;
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
            return command.ExecuteScalar()?.ToString();
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
                users.Add($"{reader.GetString(0)} ({reader.GetString(1)})");
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
                return (reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
            return ("", username, "", "");
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
                roller.Add(reader.GetString(0));
            return roller;
        }

        public static List<User> GetAllBrugere()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Navn, Username FROM Users";
            var brugere = new List<User>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                brugere.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Navn = reader.GetString(1),
                    Username = reader.GetString(2)
                });
            }
            return brugere;
        }
    }
}