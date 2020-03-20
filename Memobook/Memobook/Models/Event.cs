using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Memobook
{
    public class Event
    {
        [PrimaryKey, AutoIncrement]
        public int EventIdAuto { get; set; }
        [MaxLength(30)]
        public string EventId { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Status { get; set; }
        public string Connection_string { get; set; }
        public int Mine { get; set; }
        public int Selected { get; set; }
    }
}
