using _2.semEksamenProjekt.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _2.semEksamenProjekt.Windows
{
    public partial class Bruger_Window : Window
    {
        private int _brugerId;
        private bool _erRedigering;

        // Opret ny bruger
        public Bruger_Window()
        {
            InitializeComponent();
            _erRedigering = false;
            TxtTitel.Text = "Opret bruger";
            BtnGem.Content = "Opret";
        }

        // Rediger eksisterende bruger
        public Bruger_Window(int brugerId, string navn, string brugernavn, string password, string rolle)
        {
            InitializeComponent();
            _erRedigering = true;
            _brugerId = brugerId;
            TxtTitel.Text = "Rediger bruger";
            BtnGem.Content = "Gem ændringer";

            TxtNavn.Text = navn;
            TxtBrugernavn.Text = brugernavn;
            TxtPassword.Text = password;

            if (rolle == "Elev") RbElev.IsChecked = true;
            else if (rolle == "Lærer") RbLærer.IsChecked = true;
            else if (rolle == "Admin") RbAdmin.IsChecked = true;
        }

        private void BtnGem_Click(object sender, RoutedEventArgs e)
        {
            string navn = TxtNavn.Text;
            string brugernavn = TxtBrugernavn.Text;
            string password = TxtPassword.Text;

            string rolle = "";
            if (RbElev.IsChecked == true) rolle = "Elev";
            else if (RbLærer.IsChecked == true) rolle = "Lærer";
            else if (RbAdmin.IsChecked == true) rolle = "Admin";

            if (string.IsNullOrWhiteSpace(navn) || string.IsNullOrWhiteSpace(brugernavn) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(rolle))
            {
                MessageBox.Show("Udfyld alle felter");
                return;
            }

            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();

            if (_erRedigering)
            {
                kommando.CommandText = @"
                    UPDATE Brugere 
                    SET Navn = $navn, Brugernavn = $brugernavn, Password = $password, Rolle = $rolle
                    WHERE Id = $id
                ";
                kommando.Parameters.AddWithValue("$id", _brugerId);
            }
            else
            {
                kommando.CommandText = @"
                    INSERT INTO Brugere (Navn, Brugernavn, Password, Rolle)
                    VALUES ($navn, $brugernavn, $password, $rolle)
                ";
            }

            kommando.Parameters.AddWithValue("$navn", navn);
            kommando.Parameters.AddWithValue("$brugernavn", brugernavn);
            kommando.Parameters.AddWithValue("$password", password);
            kommando.Parameters.AddWithValue("$rolle", rolle);
            kommando.ExecuteNonQuery();

            MessageBox.Show(_erRedigering ? "Bruger opdateret!" : "Bruger oprettet!");
            this.Close();
        }
    }
}