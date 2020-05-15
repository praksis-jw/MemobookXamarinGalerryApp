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
            await Navigation.PushAsync(new QrCodeShow(qrcode));
        }


        

        protected override void OnAppearing()
        {
            base.OnAppearing();

           
            conn = DependencyService.Get<ISQLite>().GetConnection();
            EventUser a = conn.Table<EventUser>().Where(k => k.EventId == EventId).ToList()[0];
            lNazwaWyd.Text = a.Name;
            lDataStartu.Text = a.StartDate;
            lDataKonca.Text = a.EndDate;
            qrcode = a.qrcode;


            var stream = DependencyService.Get<IBarcodeService>().ConvertImageStream(qrcode,300,300);
            QrButton.Source = ImageSource.FromStream(() => { return stream; });
            conn.CreateTable<EventPhoto>();

            conn.CreateTable<EventPhoto>();
            List<EventPhoto> query = conn.Query<EventPhoto>("Select * From EventPhoto where EventId = '" + EventId + "'"
                             );

            for (int col = 0; col < query.Count; ++col)
            {

                if (query[col].EventPhotoId ==0)
                {
                    var child = new Image {Source = "Spinner.gif", IsAnimationPlaying = true, StyleId=query[col].PhotoId.ToString() };

                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += (s, e) => {
                       
                        Navigation.PushAsync(new PhotoShow(int.Parse(query[col].EventId)));

                    };
                    child.GestureRecognizers.Add(tapGestureRecognizer);
                    grid.Children.Add(new Frame { CornerRadius = 0,  Padding = 0, Content= child } , col % 4, col / 4);
                    // new Image { Source = "Spinner.gif", IsAnimationPlaying = true }
                }
                else
                {
                    var stream1 = new MemoryStream(query[col].Photo);
                    var child = new Image {   Source = ImageSource.FromStream(() => stream1), StyleId = query[col].PhotoId.ToString() };

                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += (s, e) => {
                        var a1 = s as Image;
                        Navigation.PushAsync(new PhotoShow(int.Parse(a1.StyleId)));

                    };
                    child.GestureRecognizers.Add(tapGestureRecognizer);

                    grid.Children.Add(new Frame { CornerRadius = 0, Padding = 0, Content = child }, col % 4, col / 4);
                }
            }
        }
    }
}