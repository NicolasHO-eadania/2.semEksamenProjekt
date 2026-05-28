using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _2.semEksamenProjekt
{
    public partial class EventOverviewWindow : Window
    {
        double timeHeight = 60; // pixels pr time vertikalt
        double dayWidth = 150; // pixels pr dag horisontalt
        int days = 7; // dage på skemaet
        int startHour = 8; // hvornår skemaet starter
        int endHour = 16; // hvornår skemaet slutter

        EventOverview overview = new EventOverview();

        public EventOverviewWindow()
        {
            InitializeComponent();
        }

        // laver skemaet
        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // størrelse på canvas
            int totalHours = endHour - startHour;
            EventsCanvas.Width  = days * dayWidth;
            EventsCanvas.Height = totalHours * timeHeight;

            AddSampleData();

            DrawTimeLabels();

            DrawGrid();

            foreach (Event ev in overview.AllEvents)
            {
                DrawEvent(ev);
            }
        }


        // eksempler
        public void AddSampleData()
        {
            DateTime monday = GetMonday(DateTime.Today);

            User teacherKenneth = new User { username = "Kenneth", role = "Underviser" };
            User teacherDenni   = new User { username = "Denni",  role = "Underviser" };

            Team classA = new Team
            {
                teamName = "sibdat25",
                year = 2025,
                education = "Datamatiker",
                city = "Silkeborg"
            };

            new Event
            {
                title = "Teknologi",
                start = monday.AddHours(8),
                end = monday.AddHours(10),
                rooms = new List<string> { "Lokale A101" },
                teachers = new List<User> { teacherKenneth },
                teams = new List<Team> { classA },
                tags = new List<string> { "Undervisning" }
            }.AddEvent(overview);

            new Event
            {
                title = "UI og UX",
                start = monday.AddDays(1).AddHours(9),
                end = monday.AddDays(1).AddHours(11),
                rooms = new List<string> { "A202" },
                teachers = new List<User> { teacherDenni },
                teams = new List<Team> { classA },
                tags = new List<string> { "Undervisning" }
            }.AddEvent(overview);
        }

        public void DrawTimeLabels()
        {
            for (int hour = startHour; hour <= endHour; hour++)
            {
                TextBlock label = new TextBlock
                {
                    Text = $"{hour:D2}:00",
                    Width = 55,
                    Height = timeHeight,
                    TextAlignment = TextAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Padding = new Thickness(0, 20, 5, 0),
                    FontSize = 11
                };
                TimeColumn.Children.Add(label);
            }
        }

        // linjer i skemaet
        public void DrawGrid()
        {
            int totalHours = endHour - startHour;

            // en vandret linje for hver time
            for (int i = 0; i <= totalHours; i++)
            {
                Line line = new Line
                {
                    X1 = 0,
                    Y1 = i * timeHeight,
                    X2 = days * dayWidth,
                    Y2 = i * timeHeight,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                EventsCanvas.Children.Add(line);
            }

            // en lodret linje for hver dag
            for (int i = 0; i <= days; i++)
            {
                Line line = new Line
                {
                    X1 = i * dayWidth,
                    Y1 = 0,
                    X2 = i * dayWidth,
                    Y2 = totalHours * timeHeight,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 1
                };
                EventsCanvas.Children.Add(line);
            }
        }

        public void DrawEvent(Event ev)
        {
            // finder den rigtige dag i skemaet
            int dayIndex = GetDayIndex(ev.start);

            // crash fix
            if (dayIndex < 0)
                return;

            // y position baseret på starttidspunkt
            double topPos = (ev.start.Hour - startHour + ev.start.Minute / 60.0) * timeHeight;

            // højde baseret på varighed
            double durationHours = (ev.end - ev.start).TotalHours;
            double blockHeight   = durationHours * timeHeight - 4;

            // opretter event blokken
            Border block = new Border
            {
                Width = dayWidth - 6,
                Height = blockHeight,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1)
            };

            StackPanel content = new StackPanel { Margin = new Thickness(5, 3, 5, 3) };

            // titel
            content.Children.Add(new TextBlock
            {
                Text = ev.title,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 12
            });

            // tidspunkt
            content.Children.Add(new TextBlock
            {
                Text = $"{ev.start:HH:mm} – {ev.end:HH:mm}",
                FontSize = 11
            });

            // lokale
            if (ev.rooms != null && ev.rooms.Count > 0)
            {
                content.Children.Add(new TextBlock
                {
                    Text = ev.rooms[0],
                    FontSize = 10
                });
            }

            // undervisere
            if (ev.teachers != null && ev.teachers.Count > 0)
            {
                content.Children.Add(new TextBlock
                {
                    Text = ev.teachers[0].username,
                    FontSize = 10
                });
            }

            block.Child = content;

            Canvas.SetLeft(block, dayIndex * dayWidth + 3);
            Canvas.SetTop(block, topPos + 2);

            EventsCanvas.Children.Add(block);
        }

        // fik hjælp fra claude----------------------------------------------------------------------------------------------
        public DateTime GetMonday(DateTime date)
        {
            int daysSinceMonday = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return date.AddDays(-daysSinceMonday).Date;
        }

        // fik hjælp fra claude ----------------------------------------------------------------------------------------------
        public int GetDayIndex(DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday    => 0,
                DayOfWeek.Tuesday   => 1,
                DayOfWeek.Wednesday => 2,
                DayOfWeek.Thursday  => 3,
                DayOfWeek.Friday    => 4,
                DayOfWeek.Saturday  => 5,
                DayOfWeek.Sunday    => 6,
                _                   => -1
            };
        }
    }
}
