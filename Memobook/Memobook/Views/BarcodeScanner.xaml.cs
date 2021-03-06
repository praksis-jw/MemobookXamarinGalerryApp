﻿using Memobook.Data;
using Memobook.Interfaces;
using ModernHttpClient;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
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
        public string komunikat { get; set; }
        public EventUser x123 { get; set; }
        public BarcodeScanner()
        {
            _isScanning = true;
            InitializeComponent();
     
        }

        private bool _isScanning { get; set; }

        public async void Handle_OnScanResult(Result result)
        {

          

                if (_isScanning == true)
            {

                _isScanning = false;

                //DisplayAlert("Scanned result", result.Text, "OK");

                qrcode = result.Text;


                string eventid;
                string onedriveid;

                Encryption x = new Encryption();
                x.SecretDecryptor(qrcode, out eventid, out onedriveid);

                conn = DependencyService.Get<ISQLite>().GetConnection(); 
                List<EventUser> query = conn.Query<EventUser>("Select * From EventUser where UPPER(eventid) = '" + eventid + "'");


                if (query.Count > 0)
                {
                    komunikat = "jesteś już członkiem tego wydarzenia";
                    Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());

                }
                else
                {
                    komunikat = "zostałeś dodany do wydarzenia";


                    using (var client = new HttpClient())
                {

                     //client.BaseAddress = new Uri("https://pph-ws.azurewebsites.net/Event/AddUserToEvent/");

                        client.BaseAddress = new Uri(App.UrlStart + "Event/AddUserToEvent/");

                        client.DefaultRequestHeaders.Authorization
                                 = new AuthenticationHeaderValue("basic", qrcode);

                        var response = await client.GetAsync("aa12345");

                        if (response.IsSuccessStatusCode)
                        {
                            var str = await response.Content.ReadAsStringAsync();
                            //udało się dodac wydarzenie nalezy je teraz wrzucić do bazy wewnętrznej.

                            EventUser dodanyevent = new EventUser();
                            //var JsonObject = JsonConvert.DeserializeObject(str);

                            List<EventUser> UserList = JsonConvert.DeserializeObject<List<EventUser>>(str);

                            conn = DependencyService.Get<ISQLite>().GetConnection();
                            dodanyevent = UserList[0];
                            dodanyevent.Mine = false;
                            dodanyevent.qrcode = qrcode;
                            //conn.Query<EventUser>("select * from EventUser",null);
                            conn.CreateTable<EventUser>();
                            conn.Insert(dodanyevent);
                            x123 = dodanyevent;
                            Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());
                        }
                    }
                    }
                }
        }
    }
}