using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using _2.semEksamenProjekt;

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
    }
}