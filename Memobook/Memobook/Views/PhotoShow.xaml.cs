using Memobook.Data;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Memobook.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoShow : ContentPage
    {

        public SQLiteConnection conn;
        public PhotoShow(int photoId)
        {
            InitializeComponent();


            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<EventPhoto>();

            List<EventPhoto> query = conn.Query<EventPhoto>("Select * From EventPhoto where PhotoId = " + photoId );


            var stream1 = new MemoryStream(query[0].PhotoOriginal);

            photo.Source = ImageSource.FromStream(() => stream1);
        }
    }
}