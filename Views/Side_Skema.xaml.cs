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

            LoadFlows();
            LoadFlowOversigt();
        }

        private void LoadFlows()
        {
            FlowTree.Items.Clear();
            var flows = FlowDbService.GetFlowsForUser(_username);
            foreach (var flow in flows)
            {
                var flowNode = new TreeViewItem
                {
                    Header = flow,
                    FontWeight = FontWeights.Bold,
                    Tag = new { Type = "flow", Navn = flow }
                };

                var underflows = FlowDbService.GetUnderflows(flow);
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


        private void OpretUnderflow_Click(object sender, RoutedEventArgs e)
        {
            if (FlowTree.SelectedItem is not TreeViewItem valgt) return;
            if (string.IsNullOrEmpty(UnderflowTitelBox.Text)) return;

            dynamic tag = valgt.Tag;
            string flowNavn = tag.Navn;
            int flowId = FlowDbService.GetFlowId(flowNavn);

            FlowDbService.OpretUnderflow(UnderflowTitelBox.Text, UnderflowTekstBox.Text, flowId);
            UnderflowTitelBox.Text = "";
            UnderflowTekstBox.Text = "";
            LoadFlows();
        }

        private void LoadFlowOversigt()
        {
            var liste = new List<FlowOversigtsRække>();
            var flows = FlowDbService.GetFlowsForUser(_username);

            foreach (var flow in flows)
            {
                var underflows = FlowDbService.GetUnderflows(flow);
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
}
