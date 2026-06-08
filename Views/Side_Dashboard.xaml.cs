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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2.semEksamenProjekt.Views
{
    /// <summary>
    /// Interaction logic for Side_Dashboard.xaml
    /// </summary>
    public partial class Side_Dashboard : UserControl
    {
        public Side_Dashboard()
        {
            InitializeComponent();
            IndlæsBrugere();
            IndlæsFlows();
        }

        private void IndlæsBrugere()
        {
            BrugerListe.ItemsSource = Database.GetAllUsers();
        }

        private void OpretBruger_Click(object sender, RoutedEventArgs e)
        {
            Pop_Up_Window_OpretRedigerBruger vindue = new Pop_Up_Window_OpretRedigerBruger();
            vindue.ShowDialog();
            IndlæsBrugere();
        }

        private void RedigerBruger_Click(object sender, RoutedEventArgs e)
        {
            if (BrugerListe.SelectedItem == null)
            {
                MessageBox.Show("Vælg en bruger for at redigere.");
                return;
            }

            string valgt = BrugerListe.SelectedItem.ToString();
            string brugernavn = valgt.Split(' ')[0];

            Pop_Up_Window_OpretRedigerBruger vindue = new Pop_Up_Window_OpretRedigerBruger(brugernavn);
            vindue.ShowDialog();
            IndlæsBrugere();
        }

        private void SletBruger_Click(object sender, RoutedEventArgs e)
        {
            if (BrugerListe.SelectedItem == null)
            {
                MessageBox.Show("Vælg en bruger for at slette.");
                return;
            }

            string valgt = BrugerListe.SelectedItem.ToString();
            string brugernavn = valgt.Split(' ')[0];

            MessageBoxResult resultat = MessageBox.Show(
                $"Er du sikker på at du vil slette brugeren '{brugernavn}'?",
                "Slet Bruger",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultat == MessageBoxResult.Yes)
            {
                Database.DeleteUser(brugernavn);
                IndlæsBrugere();
            }
        }

        private void IndlæsFlows()
        {
            FlowListe.ItemsSource = Database.GetAllFlows();
        }

        private void OpretFlow_Click(object sender, RoutedEventArgs e)
        {
            Pop_Up_Window_OpretFlow vindue = new Pop_Up_Window_OpretFlow();
            vindue.ShowDialog();
            IndlæsFlows();
        }

        private void RedigerFlow_Click(object sender, RoutedEventArgs e)
        {
            if (FlowListe.SelectedItem == null)
            {
                MessageBox.Show("Vælg et flow for at redigere.");
                return;
            }

            string flow = FlowListe.SelectedItem.ToString();
            Pop_Up_Window_OpretFlow vindue = new Pop_Up_Window_OpretFlow(flow);
            vindue.ShowDialog();
            IndlæsFlows();
        }

        private void SletFlow_Click(object sender, RoutedEventArgs e)
        {
            if (FlowListe.SelectedItem == null)
            {
                MessageBox.Show("Vælg et flow for at slette.");
                return;
            }

            string flow = FlowListe.SelectedItem.ToString();

            MessageBoxResult resultat = MessageBox.Show(
                $"Er du sikker på at du vil slette flowet '{flow}'?",
                "Slet Flow",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultat == MessageBoxResult.Yes)
            {
                Database.DeleteFlow(flow);
                IndlæsFlows();
            }
        }

        private void ManageFlow_Click(object sender, RoutedEventArgs e)
        {
            Pop_Up_Window_ManageFlow vindue = new Pop_Up_Window_ManageFlow();
            vindue.ShowDialog();
        }
    }
}
