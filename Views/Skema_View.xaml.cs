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
    public partial class Skema_View : UserControl
    {
        private DateTime _mandag;

        public Skema_View()
        {
            InitializeComponent();
            _mandag = StartAfUge(DateTime.Today);
            OpdaterUgeLabel();
        }

        private void BtnForrige_Click(object sender, RoutedEventArgs e)
        {
            _mandag = _mandag.AddDays(-7);
            OpdaterUgeLabel();
        }

        private void BtnNæste_Click(object sender, RoutedEventArgs e)
        {
            _mandag = _mandag.AddDays(7);
            OpdaterUgeLabel();
        }

        private void OpdaterUgeLabel()
        {
            int ugeNr = System.Globalization.ISOWeek.GetWeekOfYear(_mandag);
            TxtUge.Text = $"Uge {ugeNr}  –  {_mandag:dd. MMM} til {_mandag.AddDays(4):dd. MMM yyyy}";

            TxtMan.Text = "Mandag\n" + _mandag.ToString("dd/MM");
            TxtTir.Text = "Tirsdag\n" + _mandag.AddDays(1).ToString("dd/MM");
            TxtOns.Text = "Onsdag\n" + _mandag.AddDays(2).ToString("dd/MM");
            TxtTor.Text = "Torsdag\n" + _mandag.AddDays(3).ToString("dd/MM");
            TxtFre.Text = "Fredag\n" + _mandag.AddDays(4).ToString("dd/MM");

            IndlæsLektioner();
        }

        public void IndlæsMineFlows()
        {
            var main = (MainWindow)Application.Current.MainWindow;
            int brugerId = main.LoggetIndBrugerId;

            FlowListe.Items.Clear();

            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = @"
                SELECT f.Navn, f.Beskrivelse
                FROM FlowTilmeldinger ft
                JOIN Flows f ON f.Id = ft.FlowId
                WHERE ft.BrugerId = $brugerId
            ";
            kommando.Parameters.AddWithValue("$brugerId", brugerId);

            using var reader = kommando.ExecuteReader();
            while (reader.Read())
                FlowListe.Items.Add($"{reader.GetString(0)} – {reader.GetString(1)}");
        }

        public void IndlæsLektioner()
        {
            if (LektionsCanvas.ActualWidth == 0)
            {
                LektionsCanvas.Loaded += (s, e) => IndlæsLektioner();
                return;
            }

            var main = (MainWindow)Application.Current.MainWindow;
            int brugerId = main.LoggetIndBrugerId;

            LektionsCanvas.Children.Clear();

            using var connection = Database.HentForbindelse();
            var kommando = connection.CreateCommand();
            kommando.CommandText = @"
                SELECT l.Start, l.Slut, l.Lokale, f.Navn
                FROM Lektioner l
                JOIN Flows f ON f.Id = l.FlowId
                JOIN FlowTilmeldinger ft ON ft.FlowId = l.FlowId
                WHERE ft.BrugerId = $brugerId
            ";
            kommando.Parameters.AddWithValue("$brugerId", brugerId);

            using var reader = kommando.ExecuteReader();
            while (reader.Read())
            {
                DateTime start = DateTime.Parse(reader.GetString(0));
                DateTime slut = DateTime.Parse(reader.GetString(1));
                string lokale = reader.GetString(2);
                string flowNavn = reader.GetString(3);

                if (start.Date < _mandag || start.Date > _mandag.AddDays(4)) continue;

                int dagIndex = (int)start.DayOfWeek - 1;
                double top = (start.Hour - 8) * 60 + start.Minute;
                double height = (slut - start).TotalMinutes;
                double kolWidth = LektionsCanvas.ActualWidth / 5.0;

                var blok = new Border
                {
                    Width = kolWidth - 4,
                    Height = height,
                    Background = new SolidColorBrush(Color.FromRgb(99, 140, 232)),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(4),
                    ToolTip = $"{flowNavn}\n{start:HH:mm} – {slut:HH:mm}\nLokale: {lokale}"
                };

                var tekst = new StackPanel();
                tekst.Children.Add(new TextBlock
                {
                    Text = flowNavn,
                    FontWeight = FontWeights.Bold,
                    FontSize = 11,
                    Foreground = Brushes.White,
                    TextTrimming = TextTrimming.CharacterEllipsis
                });
                tekst.Children.Add(new TextBlock
                {
                    Text = $"{start:HH:mm}–{slut:HH:mm}",
                    FontSize = 10,
                    Foreground = Brushes.White
                });
                tekst.Children.Add(new TextBlock
                {
                    Text = $"📍 {lokale}",
                    FontSize = 10,
                    Foreground = Brushes.White
                });

                blok.Child = tekst;

                Canvas.SetLeft(blok, dagIndex * kolWidth + 2);
                Canvas.SetTop(blok, top);
                LektionsCanvas.Children.Add(blok);
            }
        }

        private static DateTime StartAfUge(DateTime d)
        {
            int dag = (int)d.DayOfWeek;
            return d.AddDays(dag == 0 ? -6 : 1 - dag).Date;
        }
    }
}
