using System;
using System.Collections.Generic;
using System.Text;
using SQLite;


namespace Memobook
{
    public class EventUser
    {
        public Guid EventUserID { get; set; }
        public Guid EventID { get; set; }
        public string Name { get; set; }
        public int StatusId { get; set; }
        public int Owner { get; set; }
    }
}
