using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace _2.semEksamenProjekt.Services
{
    public class Database
    {
        public static string _sti = "Data Source=skole.db";

        public static void Initialiser()
        {
            using var connection = new SqliteConnection(_sti);
            connection.Open();

            var kommando = connection.CreateCommand();
            kommando.CommandText = "PRAGMA journal_mode=WAL;";
            kommando.ExecuteNonQuery();

            kommando.CommandText = @"
        CREATE TABLE IF NOT EXISTS Brugere (
            Id          INTEGER PRIMARY KEY AUTOINCREMENT,
            Navn        TEXT NOT NULL,
            Brugernavn  TEXT NOT NULL,
            Password    TEXT NOT NULL,
            Rolle       TEXT NOT NULL
        );";
            kommando.ExecuteNonQuery();

            kommando.CommandText = @"
        CREATE TABLE IF NOT EXISTS Flows (
            Id          INTEGER PRIMARY KEY AUTOINCREMENT,
            Navn        TEXT NOT NULL,
            Beskrivelse TEXT
        );";
            kommando.ExecuteNonQuery();

            kommando.CommandText = @"
        CREATE TABLE IF NOT EXISTS Lektioner (
            Id          INTEGER PRIMARY KEY AUTOINCREMENT,
            FlowId      INTEGER NOT NULL,
            Lokale      TEXT NOT NULL,
            Start       TEXT NOT NULL,
            Slut        TEXT NOT NULL,
            FOREIGN KEY (FlowId) REFERENCES Flows(Id)
        );";
            kommando.ExecuteNonQuery();

            kommando.CommandText = @"
        CREATE TABLE IF NOT EXISTS FlowTilmeldinger (
            Id        INTEGER PRIMARY KEY AUTOINCREMENT,
            FlowId    INTEGER NOT NULL,
            BrugerId  INTEGER NOT NULL,
            FOREIGN KEY (FlowId)   REFERENCES Flows(Id),
            FOREIGN KEY (BrugerId) REFERENCES Brugere(Id)
        );";
            kommando.ExecuteNonQuery();

            kommando.CommandText = @"
        INSERT OR IGNORE INTO Brugere (Id, Navn, Brugernavn, Password, Rolle)
        VALUES 
            (1, 'Admin Hansen', 'admin',  '1234', 'Admin'),
            (2, 'Lars Lærer',   'laerer', '1234', 'Lærer'),
            (3, 'Emma Elev',    'elev',   '1234', 'Elev');";
            kommando.ExecuteNonQuery();
        }

        public static SqliteConnection HentForbindelse()
        {
            var connecntion = new SqliteConnection(_sti);
            connecntion.Open();
            return connecntion;
        }
    }
}
