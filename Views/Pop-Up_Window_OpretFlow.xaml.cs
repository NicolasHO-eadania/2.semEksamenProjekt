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

namespace _2.semEksamenProjekt
{
    public partial class Pop_Up_Window_OpretFlow : Window
    {
        private readonly string _oprindeligtNavn;
        private readonly bool _erRedigering;

        public Pop_Up_Window_OpretFlow()
        {
            InitializeComponent();
            _erRedigering = false;
        }

        public Pop_Up_Window_OpretFlow(string navn)
        {
            InitializeComponent();
            _erRedigering = true;
            _oprindeligtNavn = navn;

            var flow = Database.GetFlow(navn);
            Flow_Titel.Text = flow.navn;
            Flow_Beskrivelse.Text = flow.beskrivelse;
        }

        private void FlowGem_Click(object sender, RoutedEventArgs e)
        {
            string titel = Flow_Titel.Text;
            string beskrivelse = Flow_Beskrivelse.Text;

            if (string.IsNullOrWhiteSpace(titel))
            {
                MessageBox.Show("Titel må ikke være tom!");
                return;
            }

            if (_erRedigering)
            {
                Database.UpdateFlow(_oprindeligtNavn, titel, beskrivelse);
                MessageBox.Show("Flow opdateret!");
            }
            else
            {
                Database.CreateFlow(titel, beskrivelse);
                MessageBox.Show("Flow oprettet!");
            }

            this.Close();
        }
    }
}