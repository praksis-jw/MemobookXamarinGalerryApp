using Memobook.Data;
using Memobook.Interfaces;
using Memobook.Views;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public string qrcode { get; set; }
        public SQLiteConnection conn { get; set; }

        public ObservableCollection<EventPhoto> PhotosList { get; set; }
        public EventInfo(string EventID1)
        {
            InitializeComponent();

            EventId = EventID1;

            // ImageSource.FromStream(() => new MemoryStream(imageAsBytes));
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new QrCodeShow(qrcode));
        }

    

    protected override void OnAppearing()
        {
            base.OnAppearing();
   

            conn = DependencyService.Get<ISQLite>().GetConnection();

            //PhotosList = new ObservableCollection<EventPhoto>(conn.Table<EventPhoto>().ToList());
            EventUser a = conn.Table<EventUser>().Where(k => k.EventId == EventId).ToList()[0];
            lNazwaWyd.Text = a.Name;
            lDataStartu.Text = a.StartDate;
            lDataKonca.Text = a.EndDate;
            qrcode = a.qrcode;



            conn.CreateTable<EventPhoto>();
            List<EventPhoto> query = conn.Query<EventPhoto>("Select * From EventPhoto where EventId = '" + EventId + "'"
                             );
            List<EventPhoto> query2 = conn.Query<EventPhoto>("Select * From EventPhoto"
                            );

            // Create cols for each img
            //for (int col = 0; col < query.Count; ++col)
            //{
            //    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            //}
            // Populate grid

            for (int col = 0; col < query.Count; ++col)
            {

                if (query[col].PhotoOriginal is null)
                {
                    grid.Children.Add(new Image { Source = null }, 0, col);
                }
                else
                {
                    var stream1 = new MemoryStream(query[col].PhotoOriginal);
                    grid.Children.Add(new Image { Source = ImageSource.FromStream(() => stream1) }, col % 4, col / 4);
                }
            }
        }
    }
}