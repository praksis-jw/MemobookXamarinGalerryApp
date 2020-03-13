using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Memobook.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
