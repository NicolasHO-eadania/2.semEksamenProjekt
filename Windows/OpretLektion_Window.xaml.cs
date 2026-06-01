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

namespace _2.semEksamenProjekt.Windows
{
    public partial class OpretLektion_Window : Window
    {
        public OpretLektion_Window()
        {
            InitializeComponent();
            IndlæsFlows();
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

        private void BtnOpret_Click(object sender, RoutedEventArgs e)
        {
            if (CmbFlow.SelectedItem is not ComboBoxItem flow)
            {
                MessageBox.Show("Vælg et flow");
                return;
            }

            if (DatoPicker.SelectedDate == null)
            {
                MessageBox.Show("Vælg en dato");
                return;
            }

            string lokale = TxtLokale.Text;
            string start = TxtStart.Text;
            string slut = TxtSlut.Text;

            if (string.IsNullOrWhiteSpace(lokale) ||
                string.IsNullOrWhiteSpace(start) ||
                string.IsNullOrWhiteSpace(slut))
            {
                MessageBox.Show("Udfyld alle felter");
                return;
            }

            DateTime dato = DatoPicker.SelectedDate.Value;
            string startDato = $"{dato:yyyy-MM-dd} {start}:00";
            string slutDato = $"{dato:yyyy-MM-dd} {slut}:00";
            int flowId = (int)flow.Tag;

            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = @"
                INSERT INTO Lektioner (FlowId, Lokale, Start, Slut)
                VALUES ($flowId, $lokale, $start, $slut)
            ";
            kommando.Parameters.AddWithValue("$flowId", flowId);
            kommando.Parameters.AddWithValue("$lokale", lokale);
            kommando.Parameters.AddWithValue("$start", startDato);
            kommando.Parameters.AddWithValue("$slut", slutDato);
            kommando.ExecuteNonQuery();

            MessageBox.Show("Lektion oprettet!");
            this.Close();
        }
    }
}