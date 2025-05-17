using System;
using System.Collections.Generic;

namespace CalendarProject
{
    public class Event
    {
        // Properties
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Owner { get; set; }

        public TimeSpan Duration
        {
            get { return EndTime - StartTime; }
        }

        // constructor
        public Event(string title, DateTime startTime, DateTime endTime)
        {
            // unique ID
            Id = DateTime.Now.GetHashCode() & 0x7FFFFFFF;
            Title = title;
            StartTime = startTime;
            EndTime = endTime;
        }
        public override string ToString()
        {
            return $"{Title} - {StartTime:g} to {EndTime:g}";
        }
    }
}
