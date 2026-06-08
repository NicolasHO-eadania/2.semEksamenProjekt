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

using _2.semEksamenProjekt.Services;
using System.Windows;
using System.Windows.Controls;

namespace _2.semEksamenProjekt
{
    public partial class Pop_Up_Window_ManageFlow : Window
    {
        public Pop_Up_Window_ManageFlow()
        {
            InitializeComponent();
            IndlæsFlows();
            IndlæsBrugere();

            if (FlowComboBox.Items.Count > 0)
                FlowComboBox.SelectedIndex = 0;
        }

        private void IndlæsFlows()
        {
            FlowComboBox.ItemsSource = Database.GetAllFlows();
        }

        private void IndlæsBrugere()
        {
            BrugerComboBox.ItemsSource = Database.GetAllBrugere();
        }

        private void FlowComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FlowComboBox.SelectedItem != null)
            {
                string flow = FlowComboBox.SelectedItem.ToString();
                MedlemmerListe.ItemsSource = Database.GetUsersForFlow(flow);
            }
        }

        private void Tilmeld_Click(object sender, RoutedEventArgs e)
        {
            if (FlowComboBox.SelectedItem == null || BrugerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Vælg både et flow og en bruger!");
                return;
            }

            string flow = FlowComboBox.SelectedItem.ToString();
            Database.Bruger bruger = (Database.Bruger)BrugerComboBox.SelectedItem;

            Database.TilmeldFlow(bruger.Username, flow);
            MedlemmerListe.ItemsSource = null;
            MedlemmerListe.ItemsSource = Database.GetUsersForFlow(flow);
            MessageBox.Show($"{bruger.Navn} er tilmeldt {flow}!");
        }

        private void Afmeld_Click(object sender, RoutedEventArgs e)
        {
            if (FlowComboBox.SelectedItem == null || MedlemmerListe.SelectedItem == null)
            {
                MessageBox.Show("Vælg et flow og en bruger fra medlemslisten!");
                return;
            }

            string flow = FlowComboBox.SelectedItem.ToString();
            string valgt = MedlemmerListe.SelectedItem.ToString();
            string brugernavn = valgt.Split('(', ')')[1];

            Database.AfmeldFlow(brugernavn, flow);
            MedlemmerListe.ItemsSource = Database.GetUsersForFlow(flow);
            MessageBox.Show($"{brugernavn} er afmeldt {flow}!");
        }
    }
}