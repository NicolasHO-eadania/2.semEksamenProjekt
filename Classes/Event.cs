using System;
using System.Collections.Generic;

namespace _2.semEksamenProjekt
{
    public class Event
    {
        public string title;
        public string[] description;
        public List<string> rooms;
        public DateTime start;
        public DateTime end;
        public Flow flowLink;
        public List<User> teachers;
        public string city;
        public List<Team> teams;
        public List<string> tags;

        public void AddEvent(EventOverview overview)
        {
            overview.AllEvents.Add(this);
        }

        public void DeleteEvent(EventOverview overview)
        {
            overview.AllEvents.Remove(this);
        }

        public void EditEvent(EventOverview overview, Event oldEvent)
        {

        }
    }
}
