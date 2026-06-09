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

namespace _2.semEksamenProjekt.Views
{
    public partial class Side_Login : UserControl
    {
        private readonly IUserService _userService;
        public Side_Login()
        {
            InitializeComponent();
            _userService = new UserServiceAdapter();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (_userService.Login(username, password))
            {
                string rolle = _userService.GetRole(username, password);
                MainWindow main = (MainWindow)Application.Current.MainWindow;
                main.Side_Login.Visibility = Visibility.Collapsed;

                if (rolle == "Administrativ")
                {
                    main.Side_Dashboard.Visibility = Visibility.Visible;
                }
                else
                {
                     main.Side_Skema.Visibility = Visibility.Visible;
                     main.Side_Skema.Init(username, rolle);
                }
            }
            else
            {
                MessageBox.Show("Forkert brugernavn eller password!");
            }
        }
    }
}
