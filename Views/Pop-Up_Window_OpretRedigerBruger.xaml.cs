using _2.semEksamenProjekt.Services;
using System.Windows;

namespace _2.semEksamenProjekt
{
    public partial class Pop_Up_Window_OpretRedigerBruger : Window
    {
        private readonly string _oprindeligtBrugernavn;
        private readonly bool _erRedigering;

        public Pop_Up_Window_OpretRedigerBruger()
        {
            InitializeComponent();
            _erRedigering = false;
        }

        public Pop_Up_Window_OpretRedigerBruger(string brugernavn)
        {
            InitializeComponent();
            _erRedigering = true;
            _oprindeligtBrugernavn = brugernavn;

            var data = UserService.GetUser(brugernavn);
            NavnBox.Text = data.navn;
            OpretBrugerBox.Text = data.username;
            AdgangskodeBox.Text = data.password;

            if (data.rolle == "Administrativ") RolleAdministrativ.IsChecked = true;
            else if (data.rolle == "Lærer") RolleLærer.IsChecked = true;
            else if (data.rolle == "Elev") RolleElev.IsChecked = true;
        }

        private void GemBruger_Click(object sender, RoutedEventArgs e)
        {
            string navn = NavnBox.Text;
            string username = OpretBrugerBox.Text;
            string password = AdgangskodeBox.Text;

            string rolle = "";
            if (RolleAdministrativ.IsChecked == true) rolle = "Administrativ";
            else if (RolleLærer.IsChecked == true) rolle = "Lærer";
            else if (RolleElev.IsChecked == true) rolle = "Elev";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(rolle))
            {
                MessageBox.Show("Udfyld alle felter og vælg en rolle!");
                return;
            }

            if (_erRedigering)
            {
                UserService.UpdateUser(_oprindeligtBrugernavn, navn, username, password, rolle);
                MessageBox.Show("Bruger opdateret!");
            }
            else
            {
                UserService.CreateUser(navn, username, password, rolle);
                MessageBox.Show("Bruger oprettet!");
            }

            this.Close();
        }
    }
}