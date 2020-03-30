using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
namespace Memobook
{
   public class EventPhoto
    {
        [PrimaryKey, AutoIncrement]
        public int PhotoId { get; set; }
        public string EventUserId { get; set; }
        public string EventId { get; set; }
        public DateTime DateAdded { get; set; }
        public byte[] Photo { get; set; }
        public byte[] PhotoOriginal { get; set; }
        public int EventPhotoId { get; set; }
    }
}
