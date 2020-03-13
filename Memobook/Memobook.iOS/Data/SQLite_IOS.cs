using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Memobook.Data;
using UIKit;
using System.IO;
using Xamarin.Forms;
using Memobook.iOS.Data;

[assembly: Dependency(typeof(SQLite_IOS))]


namespace Memobook.iOS.Data
{
    public class SQLite_IOS : ISQLite
    {
        public SQLite_IOS() { }
        public SQLite.SQLiteConnection GetConnection()
        {
            var fileName = "Testdb.db3";
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentPath, "..", "Library");
            var path = Path.Combine(libraryPath, fileName);
            var connection = new SQLite.SQLiteConnection(path);
            return connection;
        }
    }
}