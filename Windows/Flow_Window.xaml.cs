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
    /// <summary>
    /// Interaction logic for Flow_Window.xaml
    /// </summary>
    public partial class Flow_Window : Window
    {
        private int _flowId;
        private bool _erRedigering;

        // Opret nyt flow
        public Flow_Window()
        {
            InitializeComponent();
            _erRedigering = false;
            TxtTitel.Text = "Opret flow";
            Gem.Content = "Opret";
        }

        // Rediger eksisterende flow
        public Flow_Window(int flowId, string navn, string beskrivelse)
        {
            InitializeComponent();
            _erRedigering = true;
            _flowId = flowId;
            TxtTitel.Text = "Rediger flow";
            Gem.Content = "Gem ændringer";

            TxtNavn.Text = navn;
            TxtBeskrivelse.Text = beskrivelse;
        }

        private void BtnGem_Click(object sender, RoutedEventArgs e)
        {
            string navn = TxtNavn.Text;
            string beskrivelse = TxtBeskrivelse.Text;

            if (string.IsNullOrWhiteSpace(navn))
            {
                MessageBox.Show("Udfyld navn");
                return;
            }

            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();

            if (_erRedigering)
            {
                kommando.CommandText = @"
                    UPDATE Flows 
                    SET Navn = $navn, Beskrivelse = $beskrivelse
                    WHERE Id = $id
                ";
                kommando.Parameters.AddWithValue("$id", _flowId);
            }
            else
            {
                kommando.CommandText = @"
                    INSERT INTO Flows (Navn, Beskrivelse)
                    VALUES ($navn, $beskrivelse)
                ";
            }

            kommando.Parameters.AddWithValue("$navn", navn);
            kommando.Parameters.AddWithValue("$beskrivelse", beskrivelse);
            kommando.ExecuteNonQuery();

            MessageBox.Show(_erRedigering ? "Flow opdateret!" : "Flow oprettet!");
            this.Close();
        }
    }
}