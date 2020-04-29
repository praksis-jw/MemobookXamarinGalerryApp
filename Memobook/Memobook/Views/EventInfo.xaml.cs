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

namespace Memobook
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventInfo : ContentPage
    {
        public string EventId { get; set; }
        public SQLiteConnection conn { get; set; }
        public EventInfo(string EventID1)
        {
            InitializeComponent();

            EventId = EventID1;

            // ImageSource.FromStream(() => new MemoryStream(imageAsBytes));
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();


            conn = DependencyService.Get<ISQLite>().GetConnection();
            List<EventPhoto> query = conn.Query<EventPhoto>("Select * From EventPhoto where EventId = '" + EventId + "'"
                             );
            List<EventPhoto> query2 = conn.Query<EventPhoto>("Select * From EventPhoto"
                            );

            // Create cols for each img
            for (int col = 0; col < query.Count; ++col)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            // Populate grid
            for (int col = 0; col < query.Count; ++col)
            {
                var stream1 = new MemoryStream(query[col].Photo);
            
                grid.Children.Add(new Image { Source = ImageSource.FromStream(() => stream1) }, 0, col);
            }
        }
    }
}