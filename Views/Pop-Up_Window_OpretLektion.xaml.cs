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

namespace _2.semEksamenProjekt.Views
{
    /// <summary>
    /// Interaction logic for Pop_Up_Window_OpretLektion.xaml
    /// </summary>
    public partial class Pop_Up_Window_OpretLektion : Window
    {
        private string _username;

        public Pop_Up_Window_OpretLektion(string username)
        {
            InitializeComponent();
            _username = username;
            LektionFlowCombo.ItemsSource = FlowDbService.GetFlowsForUser(username);
        }

        private void OpretLektion_Click(object sender, RoutedEventArgs e)
        {
            if (LektionTitelBox.Text == "" || LektionFlowCombo.SelectedItem == null ||
                LektionDagCombo.SelectedItem == null || StartTidBox.Text == "" || SlutTidBox.Text == "")
            {
                MessageBox.Show("Udfyld alle felter!");
                return;
            }

            string dag = ((ComboBoxItem)LektionDagCombo.SelectedItem).Content.ToString();

            FlowDbService.OpretLektion(
                LektionTitelBox.Text,
                LektionIndholdBox.Text,
                dag,
                StartTidBox.Text,
                SlutTidBox.Text,
                LektionFlowCombo.SelectedItem.ToString()
            );

            MessageBox.Show("Lektion oprettet!");
            this.Close();
        }
    }
}
