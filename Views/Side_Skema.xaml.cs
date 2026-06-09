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
    /// Interaction logic for Side_Skema.xaml
    /// </summary>
    public partial class Side_Skema : UserControl
    {
        private string _username;
        private string _rolle;

        public Side_Skema()
        {
            InitializeComponent();
        }

        public void Init(string username, string rolle)
        {
            _username = username;
            _rolle = rolle;

            if (rolle == "Lærer")
            {
                OpretUnderflowPanel.Visibility = Visibility.Visible;
            }

            LoadSkema();
            LoadFlows();
            LoadFlowOversigt();
        }

        private void LoadFlows()
        {
            FlowTree.Items.Clear();
            var flows = Database.GetFlowsForUser(_username);
            foreach (var flow in flows)
            {
                var flowNode = new TreeViewItem
                {
                    Header = flow,
                    FontWeight = FontWeights.Bold,
                    Tag = new { Type = "flow", Navn = flow }
                };

                var underflows = Database.GetUnderflows(flow);
                foreach (var u in underflows)
                {
                    var underflowNode = new TreeViewItem
                    {
                        Header = u.titel,
                        Tag = new { Type = "underflow", Navn = u.titel, Tekst = u.tekst, Id = u.id }
                    };
                    flowNode.Items.Add(underflowNode);
                }

                FlowTree.Items.Add(flowNode);
            }
        }

        private void OpretFlow_Click(object sender, RoutedEventArgs e)
        {
            Pop_Up_Window_OpretFlow vindue = new Pop_Up_Window_OpretFlow(_username);
            vindue.ShowDialog();
            LoadFlows();
            LoadFlowOversigt();
        }

        private void FlowTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (FlowTree.SelectedItem is not TreeViewItem valgt) return;

            dynamic tag = valgt.Tag;

            if (tag.Type == "flow")
            {
                IndholdTitel.Text = tag.Navn;
                IndholdTekst.Text = "";
                if (_rolle == "Lærer")
                    OpretUnderflowPanel.Visibility = Visibility.Visible;
            }
            else if (tag.Type == "underflow")
            {
                IndholdTitel.Text = tag.Navn;
                IndholdTekst.Text = tag.Tekst;
                OpretUnderflowPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadSkema()
        {
            SkemaGrid.Children.Clear();
            SkemaGrid.ColumnDefinitions.Clear();
            SkemaGrid.RowDefinitions.Clear();

            string[] dage = { "", "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag" };
            int startTime = 6;
            int endTime = 17;
            int rowHeight = 40;

            SkemaGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
            for (int i = 1; i < dage.Length; i++)
                SkemaGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            SkemaGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            for (int t = startTime; t <= endTime; t++)
                SkemaGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight) });

            for (int d = 1; d < dage.Length; d++)
            {
                var header = new TextBlock
                {
                    Text = dage[d],
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(header, 0);
                Grid.SetColumn(header, d);
                SkemaGrid.Children.Add(header);
            }

            for (int t = startTime; t <= endTime; t++)
            {
                int row = t - startTime + 1;

                var tidLabel = new TextBlock
                {
                    Text = $"{t:D2}:00",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 11
                };
                Grid.SetRow(tidLabel, row);
                Grid.SetColumn(tidLabel, 0);
                SkemaGrid.Children.Add(tidLabel);

                for (int d = 1; d < dage.Length; d++)
                {
                    bool arbejdstid = t >= 8 && t < 16;
                    var celle = new Border
                    {
                        Background = arbejdstid
                            ? new SolidColorBrush(Colors.White)
                            : new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                        BorderBrush = new SolidColorBrush(Colors.LightGray),
                        BorderThickness = new Thickness(0.5)
                    };
                    Grid.SetRow(celle, row);
                    Grid.SetColumn(celle, d);
                    SkemaGrid.Children.Add(celle);
                }
            }

            var lektioner = Database.GetLektionerForUser(_username);
            string[] dagNavne = { "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag" };

            foreach (var l in lektioner)
            {
                int dagIndex = Array.IndexOf(dagNavne, l.dag) + 1;
                if (dagIndex <= 0) continue;

                if (!int.TryParse(l.startTid.Split(':')[0], out int startHour)) continue;
                if (!int.TryParse(l.slutTid.Split(':')[0], out int slutHour)) continue;

                int row = startHour - startTime + 1;
                int rowSpan = Math.Max(1, slutHour - startHour);

                var lektionBoks = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(100, 149, 237)),
                    CornerRadius = new CornerRadius(4),
                    Margin = new Thickness(2),
                    Child = new TextBlock
                    {
                        Text = l.titel,
                        Foreground = new SolidColorBrush(Colors.White),
                        FontWeight = FontWeights.Bold,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(4),
                        FontSize = 11
                    }
                };

                Grid.SetRow(lektionBoks, row);
                Grid.SetColumn(lektionBoks, dagIndex);
                Grid.SetRowSpan(lektionBoks, rowSpan);
                SkemaGrid.Children.Add(lektionBoks);
            }
        }

        private void OpretLektion_Click(object sender, RoutedEventArgs e)
        {
            var vindue = new Pop_Up_Window_OpretLektion(_username);
            vindue.Owner = Window.GetWindow(this);
            vindue.ShowDialog();
            LoadSkema();
        }
        private void OpretUnderflow_Click(object sender, RoutedEventArgs e)
        {
            if (FlowTree.SelectedItem is not TreeViewItem valgt) return;
            if (string.IsNullOrEmpty(UnderflowTitelBox.Text)) return;

            dynamic tag = valgt.Tag;
            string flowNavn = tag.Navn;
            int flowId = Database.GetFlowId(flowNavn);

            Database.OpretUnderflow(UnderflowTitelBox.Text, UnderflowTekstBox.Text, flowId);
            UnderflowTitelBox.Text = "";
            UnderflowTekstBox.Text = "";
            LoadFlows();
        }

        private void LoadFlowOversigt()
        {
            var liste = new List<FlowOversigtsRække>();
            var flows = Database.GetFlowsForUser(_username);

            foreach (var flow in flows)
            {
                var underflows = Database.GetUnderflows(flow);
                if (underflows.Count == 0)
                {
                    liste.Add(new FlowOversigtsRække { FlowNavn = flow, UnderflowNavn = "-", Tekst = "-" });
                }
                else
                {
                    foreach (var u in underflows)
                    {
                        liste.Add(new FlowOversigtsRække
                        {
                            FlowNavn = flow,
                            UnderflowNavn = u.titel,
                            Tekst = u.tekst
                        });
                    }
                }
            }

            FlowOversigt.ItemsSource = liste;
        }

        public class FlowOversigtsRække
        {
            public string FlowNavn { get; set; }
            public string UnderflowNavn { get; set; }
            public string Tekst { get; set; }
        }
    }

    public class LektionVisning
    {
        public string Titel { get; set; }
        public string Dag { get; set; }
        public string StartTid { get; set; }
        public string SlutTid { get; set; }
        public string FlowNavn { get; set; }
    }
}
