using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Memobook.Interfaces;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Xamarin.Auth;
using static Memobook.Views.GeoNamesWebService;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Memobook.Data;
using SQLite;

//=c50[MnHBA44/NbWe.Ms6?lo8f2t63kg

namespace Memobook.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainWindow : ContentPage
    {
        public ImageSource BarcodeImageSource { get; set; }
        public string userName { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        BarcodeScanner _myModalPage;
        ObservableCollection<Event> events2;
        ObservableCollection<Event> events;
        public SQLiteConnection conn;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            Debug.WriteLine("Inside SelectedDateViewModel.OnPropertyChanged()");

         
            var trace =
            $"PropertyChanged Is Null: {(PropertyChanged == null ? "Yes" : "No")}";
            Debug.WriteLine(trace);

            var propertyChangedCallback = PropertyChanged;
            propertyChangedCallback?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        }

        public MainWindow()
        {
            userName = "";
            InitializeComponent();
            ButtonLogin.Clicked += ButtonLogin_Clicked;
            QR2Button.Clicked += QR2Button_Clicked;

            events = new ObservableCollection<Event>();
            SupplyLevels.ItemsSource = events;

            events2 = new ObservableCollection<Event>();
            SupplyLevels2.ItemsSource = events2;
          
          
     

        }
        private void QR2Button_Clicked(object sender, EventArgs e)
        {
            ShowModalPage();
          //  DisplayAlert("Zeskanowany QR", qrcode, "OK");

        }


        private async void ShowModalPage()
        {
            // When you want to show the modal page, just call this method
            // add the event handler for to listen for the modal popping event:
            Memobook.App.Current.ModalPopping += HandleModalPopping;
            _myModalPage = new BarcodeScanner();
            await Navigation.PushModalAsync(_myModalPage);
        }
        private string qrcode;
        public string Qrcode
        {
            get
            {
                return qrcode;
            }

            set
            {
                if (qrcode != value)
                {
                    qrcode = value;
                    OnPropertyChanged("Qrcode");
                }
            }
        }


        private void HandleModalPopping(object sender, ModalPoppingEventArgs e)
        {
            if (e.Modal == _myModalPage)
            {
                // now we can retrieve that phone number:
                Qrcode = _myModalPage.qrcode;
                _myModalPage = null;
                events2.Add(new Event() { Name = "Nowe dodane wydarzenie o nazwie...", EventId = "szczegóły wydarzenia" });
                // remember to remove the event handler:
                //  Memobook.App.Current.ModalPopping -= HandleModalPopping;
                // DisplayAlert("Zeskanowany QR", phoneNumber, "OK");

            }
        }




        private void ButtonLogin_Clicked(object sender, EventArgs e)
        {

            var authenticator = new OAuth2Authenticator(
        clientId: "251af922-5cae-4dc9-99c7-052d02158d99",
        scope: "https://graph.microsoft.com/User.Read",
        authorizeUrl: new Uri("https://login.microsoftonline.com/common/oauth2/V2.0/authorize"), // the auth URL for the service
        redirectUrl: new Uri("https://graph.microsoft.com/v1.0/me")); // the redirect URL for the service




            authenticator.Completed += OnAuthCompleted;
            authenticator.Error += OnAuthError;

            // AuthenticationState.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);


            //GeoNamesWebService geoService = new GeoNamesWebService();
            //var x = await geoService.LoginAsync();
            //var x1 = 1;




            //var stream = DependencyService.Get<IBarcodeService>().ConvertImageStream("Janek");
            //barcode.Source= ImageSource.FromStream(() => { return stream; });

        }

        public async void lvItemTapped(object sender, ItemTappedEventArgs e) { 
            
            var myListView = (ListView)sender; 
            
            var myItem = myListView.SelectedItem;


            await Navigation.PushModalAsync(new QrCodeShow());
        }


        private void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }


            if (e.IsAuthenticated)
            {

                var httpClient = new System.Net.Http.HttpClient();
                httpClient = new HttpClient();
                httpClient.Timeout = new TimeSpan(0, 4, 0);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", e.Account.Properties.First().Value);

               
              
                string requestPath = "https://graph.microsoft.com/v1.0/me";

                var response = RequestHelper.GetRequestAsync(requestPath, httpClient).Result;
                JObject jObjectResponse = JObject.Parse(response);
                
                userName = jObjectResponse["userPrincipalName"].Value<string>();


                using (var client1 = new HttpClient())
                {
                    string requestPath1 = "https://pph-ws.azurewebsites.net/Email/" + userName;

                    var response1 = RequestHelper.GetRequestAsync(requestPath1, client1).Result;
                   // JObject jObjectResponse1 = JObject.Parse(response1);


                    if (response1 != "")
                    {
                        ButtonLogin.IsVisible = false;
                        ButtonLogout.IsVisible = true;
                        jw2.IsVisible = true;
                        jw2.Text = "Zalogowano jako " + userName;
                        jw2.IsVisible = true;

                  
                        Settings s = new Settings();
                        s.OneDriveID = userName;
                        s.SecretPassword = response1;

                        conn = DependencyService.Get<ISQLite>().GetConnection();
                        conn.CreateTable<Settings>();
                        conn.Insert(s);


                        using (var client = new HttpClient())
                        {

                            //client.BaseAddress = new Uri("https://pph-ws.azurewebsites.net/Event/AddUserToEvent/");
                            client.BaseAddress = new Uri("https://localhost:44352/Event/GetMyEvents/");

                            client.DefaultRequestHeaders.Authorization
                                     = new AuthenticationHeaderValue("basic", response1);

                            var response12 = await client.GetAsync("aa12345");

                            if (response12.IsSuccessStatusCode)
                            {
                                var str = await response12.Content.ReadAsStringAsync();
                                //udało się dodac wydarzenie nalezy je teraz wrzucić do bazy wewnętrznej.

                               
                            }
                        }


                        events.Add(new Event() { Name = "Twoje wydarzenie nr 1", EventId = "szczegóły wydarzenia" });


                        await DisplayAlert("Komunikat", "Logowanie poprawne", "OK");
                    
                    }
                    else
                        await DisplayAlert("Komunikat", "Twojego Emaila nie ma w naszej bazie lub wpisałeś błędne hasło spróbuje ponownie", "OK");
                }
                //GeoNamesWebService geoService = new GeoNamesWebService();
                //var x = await geoService.GetPlacesAsync(userName);
                //var x1 = 1;


            }
        }
    }


    public class GeoNamesWebService
    {
        public GeoNamesWebService()
        {
        }

        public async Task<object> LoginAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://pph-ws.azurewebsites.net/");

                string urlParams = "{ \"html\": \"html\", \"width \": \"2\", \"height  \": \"2\", \"ratio  \": \"2\" }";

                ImageParam x = new ImageParam();
                x.height = 1;
                x.html = "aaaa";
                x.ratio = 2.0;
                x.width = 2;



                string jsonString;
                jsonString = JsonConvert.SerializeObject(x);

                StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var result = await client.PostAsync("Glossary/test12", content);
                string resultContent = await result.Content.ReadAsStringAsync();
                Console.WriteLine(resultContent);
                return resultContent;
            }
        }




        public async Task<object> GetPlacesAsync(string userName)
        {

            using (var client = new HttpClient())
            {
               
                client.BaseAddress = new Uri("https://pph-ws.azurewebsites.net/Email/");

                client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("", "basic aaa");

                var response = await client.GetAsync(userName);

                if (response.IsSuccessStatusCode)
                {
                    var str = await response.Content.ReadAsStringAsync();
                    var temp = JsonConvert.DeserializeObject(str);
                }

                return 1;

            }


            //string urlParams = "{ \"html\": \"html\", \"width \": \"2\", \"height  \": \"2\", \"ratio  \": \"2\" }";
            //var r = RequestHelper.PostRequestStringAsync("https://pph-ws.azurewebsites.net/Glossary/test12", urlParams, "application/json", new System.Net.Http.HttpClient()).Result;

            //var s = 1;

           // return 1;

        }

        public class ImageParam
        {
            public string html { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public double ratio { get; set; }
        }

        static public class RequestHelper
        {
            #region POST
            static public async Task<string> PostRequestStringAsync(string url, string urlParams, string contentType, HttpClient httpClient)
            {
                HttpResponseMessage response = await PostRequestAsync(url, urlParams, contentType, httpClient);
                string resultContent = await response.Content.ReadAsStringAsync();

                return resultContent;
            }

            static public async Task<HttpResponseHeaders> PostRequestHeadersAsync(string url, string urlParams, string contentType, HttpClient httpClient)
            {
                HttpResponseMessage response = await PostRequestAsync(url, urlParams, contentType, httpClient);
                return response.Headers;
            }

            static public async Task<HttpResponseMessage> PostRequestAsync(string url, string urlParams, string contentType, HttpClient httpClient)
            {
                var content = new StringContent(urlParams, Encoding.UTF8, contentType);
                return await httpClient.PostAsync(url, content);
            }
            #endregion POST


            static public async Task<string> GetRequestAsync(string url, HttpClient client)
            {
                return await client.GetStringAsync(url).ConfigureAwait(false);
            }

            static public async Task<byte[]> GetRequestBytesAsync(string url, HttpClient client)
            {
                return await client.GetByteArrayAsync(url).ConfigureAwait(false);
            }

            static public async Task<HttpResponseHeaders> GetRequestHeadersAsync(string url, HttpClient httpClient)
            {
                var response = await httpClient.GetAsync(url).ConfigureAwait(false);

                return response.Headers;
            }
        }

    }


}