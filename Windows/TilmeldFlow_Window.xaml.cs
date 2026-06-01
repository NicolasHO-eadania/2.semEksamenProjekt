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

using _2.semEksamenProjekt.Services;
using System.Windows;
using System.Windows.Controls;

namespace _2.semEksamenProjekt.Windows
{
    public partial class TilmeldFlow_Window : Window
    {
        public TilmeldFlow_Window()
        {
            InitializeComponent();
            IndlæsFlows();
            IndlæsBrugere();
        }

        private void IndlæsFlows()
        {
            CmbFlow.Items.Clear();
            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = "SELECT Id, Navn FROM Flows";
            using var reader = kommando.ExecuteReader();
            while (reader.Read())
            {
                CmbFlow.Items.Add(new ComboBoxItem
                {
                    Content = reader.GetString(1),
                    Tag = reader.GetInt32(0)
                });
            }
        }

        private void IndlæsBrugere()
        {
            CmbBruger.Items.Clear();
            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = "SELECT Id, Navn, Rolle FROM Brugere";
            using var reader = kommando.ExecuteReader();
            while (reader.Read())
            {
                CmbBruger.Items.Add(new ComboBoxItem
                {
                    Content = $"{reader.GetString(1)} ({reader.GetString(2)})",
                    Tag = reader.GetInt32(0)
                });
            }
        }

        private void IndlæsTilmeldte(int flowId)
        {
            TilmeldteListe.Items.Clear();
            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = @"
                SELECT b.Id, b.Navn, b.Rolle 
                FROM FlowTilmeldinger ft
                JOIN Brugere b ON b.Id = ft.BrugerId
                WHERE ft.FlowId = $flowId
            ";
            kommando.Parameters.AddWithValue("$flowId", flowId);
            using var reader = kommando.ExecuteReader();
            while (reader.Read())
            {
                TilmeldteListe.Items.Add(new ListBoxItem
                {
                    Content = $"{reader.GetString(1)} ({reader.GetString(2)})",
                    Tag = reader.GetInt32(0)
                });
            }
        }

        private void CmbFlow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbFlow.SelectedItem is ComboBoxItem item)
                IndlæsTilmeldte((int)item.Tag);
        }

        private void Tilmeld_Click(object sender, RoutedEventArgs e)
        {
            if (CmbFlow.SelectedItem is not ComboBoxItem flow ||
                CmbBruger.SelectedItem is not ComboBoxItem bruger)
            {
                MessageBox.Show("Vælg både flow og bruger");
                return;
            }

            int flowId = (int)flow.Tag;
            int brugerId = (int)bruger.Tag;

            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = @"
                INSERT OR IGNORE INTO FlowTilmeldinger (FlowId, BrugerId)
                VALUES ($flowId, $brugerId)
            ";
            kommando.Parameters.AddWithValue("$flowId", flowId);
            kommando.Parameters.AddWithValue("$brugerId", brugerId);
            kommando.ExecuteNonQuery();

            IndlæsTilmeldte(flowId);
        }

        private void Afmeld_Click(object sender, RoutedEventArgs e)
        {
            if (CmbFlow.SelectedItem is not ComboBoxItem flow ||
                TilmeldteListe.SelectedItem is not ListBoxItem bruger)
            {
                MessageBox.Show("Vælg et flow og en tilmeldt bruger");
                return;
            }

            int flowId = (int)flow.Tag;
            int brugerId = (int)bruger.Tag;

            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = @"
                DELETE FROM FlowTilmeldinger 
                WHERE FlowId = $flowId AND BrugerId = $brugerId
            ";
            kommando.Parameters.AddWithValue("$flowId", flowId);
            kommando.Parameters.AddWithValue("$brugerId", brugerId);
            kommando.ExecuteNonQuery();

            IndlæsTilmeldte(flowId);
        }
    }
}