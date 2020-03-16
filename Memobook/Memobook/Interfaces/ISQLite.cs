using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Memobook.Data
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
