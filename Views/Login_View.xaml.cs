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
    public partial class Login_View : UserControl
    {
        public Login_View()
        {
            InitializeComponent();
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            string brugernavn = BrugernavnBox.Text;
            string adgangskode = PasswordBox.Password;

            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = @"
        SELECT Rolle FROM Brugere 
        WHERE Brugernavn = $brugernavn 
        AND Password = $password
    ";
            kommando.Parameters.AddWithValue("$brugernavn", brugernavn);
            kommando.Parameters.AddWithValue("$password", adgangskode);

            var rolle = kommando.ExecuteScalar() as string;
            var main = (MainWindow)Application.Current.MainWindow;

            if (rolle == "Admin")
            {
                main.LoginSide.Visibility = Visibility.Collapsed;
                main.DashboardSide.Visibility = Visibility.Visible;
            }
            else if (rolle == "Lærer" || rolle == "Elev")
            {
                using var conn2 = Database.HentForbindelse();
                var cmd2 = conn2.CreateCommand();
                cmd2.CommandText = "SELECT Id FROM Brugere WHERE Brugernavn = $b";
                cmd2.Parameters.AddWithValue("$b", brugernavn);
                int brugerId = Convert.ToInt32(cmd2.ExecuteScalar());

                main.LoggetIndBrugerId = brugerId;
                main.LoginSide.Visibility = Visibility.Collapsed;
                main.SkemaSide.Visibility = Visibility.Visible;

                main.SkemaSide.IndlæsMineFlows();
                main.SkemaSide.IndlæsLektioner();
            }
            else
            {
                MessageBox.Show("Forkert brugernavn eller adgangskode");
            }
        }
    }
}
