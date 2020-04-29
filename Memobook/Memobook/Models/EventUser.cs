using System;
using System.Collections.Generic;
using System.Text;
using SQLite;


namespace Memobook
{
    public class EventUser
         
    {
        [PrimaryKey]
        public string EventId { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Status { get; set; }
        public bool Mine { get; set; }
        public string SecretPassword { get; set; }
        public string EventUserID { get; set; }
        public string UserName { get; set; }
        public int StatusID { get; set; }
    }
}
  