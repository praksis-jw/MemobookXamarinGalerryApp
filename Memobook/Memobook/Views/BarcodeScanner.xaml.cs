using Memobook.Data;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;

namespace Memobook.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarcodeScanner : ContentPage
    {
        public SQLiteConnection conn;
        public string qrcode { get; set; }
        public BarcodeScanner()
        {
            InitializeComponent();
     
        }

        public async void Handle_OnScanResult(Result result)
        {
            qrcode = result.Text;
            //DisplayAlert("Scanned result", result.Text, "OK");


            using (var client = new HttpClient())
            {

                //client.BaseAddress = new Uri("https://pph-ws.azurewebsites.net/Event/AddUserToEvent/");
                client.BaseAddress = new Uri("https://localhost:44352/Event/AddUserToEvent/");

                client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("basic", qrcode);

                var response = await client.GetAsync("aa12345");

                if (response.IsSuccessStatusCode)
                {
                    var str = await response.Content.ReadAsStringAsync();
                    //udało się dodac wydarzenie nalezy je teraz wrzucić do bazy wewnętrznej.

                    Event dodanyevent = new Event();
                    //var JsonObject = JsonConvert.DeserializeObject(str);

                    List<Event> UserList = JsonConvert.DeserializeObject<List<Event>>(str);


                    conn = DependencyService.Get<ISQLite>().GetConnection();
                    conn.CreateTable<Event>();
                    conn.Insert(dodanyevent);
                }
            }
            await Navigation.PopModalAsync();
        }
    }
}