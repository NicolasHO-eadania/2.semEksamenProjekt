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
using System.Windows.Navigation;
using System.Windows.Shapes;
using _2.semEksamenProjekt.Services;
using _2.semEksamenProjekt.Windows;

namespace _2.semEksamenProjekt.Views
{
    public partial class Dashboard_View : UserControl
    {
        public Dashboard_View()
        {
            InitializeComponent();
            IndlæsBrugere();
            IndlæsFlows();
        }

        // ── Brugere ──────────────────────────────────────────
        public void IndlæsBrugere()
        {
            BrugerListe.Items.Clear();
            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = "SELECT Navn, Brugernavn, Rolle FROM Brugere";
            using var reader = kommando.ExecuteReader();
            while (reader.Read())
                BrugerListe.Items.Add($"{reader.GetString(0)} ({reader.GetString(2)})");
        }

        private void OpretBruger_Click(object sender, RoutedEventArgs e)
        {
            var vindue = new Bruger_Window();
            vindue.ShowDialog();
            IndlæsBrugere();
        }

        private void RedigerBruger_Click(object sender, RoutedEventArgs e)
        {
            if (BrugerListe.SelectedItem == null)
            {
                MessageBox.Show("Vælg en bruger fra listen først");
                return;
            }

            string valgt = BrugerListe.SelectedItem.ToString();
            string navn = valgt.Split('(')[0].Trim();

            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = "SELECT Id, Navn, Brugernavn, Password, Rolle FROM Brugere WHERE Navn = $navn";
            kommando.Parameters.AddWithValue("$navn", navn);

            using var reader = kommando.ExecuteReader();
            if (reader.Read())
            {
                var vindue = new Bruger_Window(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4)
                );
                vindue.ShowDialog();
                IndlæsBrugere();
            }
        }

        private void SletBruger_Click(object sender, RoutedEventArgs e)
        {
            if (BrugerListe.SelectedItem == null)
            {
                MessageBox.Show("Vælg en bruger fra listen først");
                return;
            }

            string valgt = BrugerListe.SelectedItem.ToString();
            string navn = valgt.Split('(')[0].Trim();

            MessageBoxResult svar = MessageBox.Show($"Er du sikker på du vil slette {navn}?", "Slet bruger", MessageBoxButton.YesNo);

            if (svar == MessageBoxResult.Yes)
            {
                using var connection = Database.HentForbindelse();
                var kommando = connection.CreateCommand();
                kommando.CommandText = "DELETE FROM Brugere WHERE Navn = $navn";
                kommando.Parameters.AddWithValue("$navn", navn);
                kommando.ExecuteNonQuery();

                MessageBox.Show("Bruger slettet!");
                IndlæsBrugere();
            }
        }

        // ── Flows ─────────────────────────────────────────────
        public void IndlæsFlows()
        {
            FlowListe.Items.Clear();
            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = "SELECT Navn FROM Flows";
            using var reader = kommando.ExecuteReader();
            while (reader.Read())
                FlowListe.Items.Add(reader.GetString(0));
        }

        private void OpretFlow_Click(object sender, RoutedEventArgs e)
        {
            var vindue = new Flow_Window();
            vindue.ShowDialog();
            IndlæsFlows();
        }

        private void RedigerFlow_Click(object sender, RoutedEventArgs e)
        {
            if (FlowListe.SelectedItem == null)
            {
                MessageBox.Show("Vælg et flow fra listen først");
                return;
            }

            string navn = FlowListe.SelectedItem.ToString();

            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = "SELECT Id, Navn, Beskrivelse FROM Flows WHERE Navn = $navn";
            kommando.Parameters.AddWithValue("$navn", navn);

            using var reader = kommando.ExecuteReader();
            if (reader.Read())
            {
                var vindue = new Flow_Window(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2)
                );
                vindue.ShowDialog();
                IndlæsFlows();
            }
        }

        private void SletFlow_Click(object sender, RoutedEventArgs e)
        {
            if (FlowListe.SelectedItem == null)
            {
                MessageBox.Show("Vælg et flow fra listen først");
                return;
            }

            string navn = FlowListe.SelectedItem.ToString();

            MessageBoxResult svar = MessageBox.Show($"Er du sikker på du vil slette {navn}?", "Slet flow", MessageBoxButton.YesNo);

            if (svar == MessageBoxResult.Yes)
            {
                using var connection = Database.HentForbindelse();
                var kommando = connection.CreateCommand();
                kommando.CommandText = "DELETE FROM Flows WHERE Navn = $navn";
                kommando.Parameters.AddWithValue("$navn", navn);
                kommando.ExecuteNonQuery();

                MessageBox.Show("Flow slettet!");
                IndlæsFlows();
            }
        }

        private void TilmeldFlow_Click(object sender, RoutedEventArgs e)
        {
            var vindue = new TilmeldFlow_Window();
            vindue.ShowDialog();
        }

        private void OpretLektion_Click(object sender, RoutedEventArgs e)
        {
            var vindue = new OpretLektion_Window();
            vindue.ShowDialog();
        }
    }
}