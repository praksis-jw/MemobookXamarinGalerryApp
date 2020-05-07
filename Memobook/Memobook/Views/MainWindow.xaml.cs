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
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Memobook.Data;
using SQLite;
using System.Net;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Bson;
using System.Net.Http.Formatting;

//=c50[MnHBA44/NbWe.Ms6?lo8f2t63kg

namespace Memobook.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainWindow : ContentPage
    {
        public ImageSource BarcodeImageSource { get; set; }
        public string userName { get; set; }

        public string OneDriveId { get; set; }
        public string BusinessId { get; set; }
        public string SecretPassword { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        BarcodeScanner _myModalPage;
        EventInfo _myModalPag2e;

        public SQLiteConnection conn;

        public ObservableCollection<EventUser> events2 { get; set; }
        public ObservableCollection<EventUser> events { get; set; }
        public ObservableCollection<EventUser> events3 { get; set; }

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
            App.UrlStart = "https://192.168.100.108:45455/";
           // App.UrlStart = "https://pph-ws.azurewebsites.net/";
            userName = "";
            InitializeComponent();
            ButtonLogin.Clicked += ButtonLogin_Clicked;
            ButtonLogout.Clicked += ButtonLogout_Clicked;
            QR2Button.Clicked += QR2Button_Clicked;
            ResetDatabase.Clicked += ResetDatabase_Clicked;



            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<Settings>();
            //TODO: sprawdzić czy konto dalej ważne jeśli online.

            conn.CreateTable<EventUser>();
            var allItems = conn.Table<Settings>().ToList();

            int count = allItems.Count();
            if (count > 0)
            {
                OneDriveId = allItems[0].OneDriveID;
                SecretPassword = allItems[0].SecretPassword;



                ButtonLogin.IsVisible = false;
                ButtonLogout.IsVisible = true;
                jw2.IsVisible = true;
                jw2.Text = "Zalogowano jako " + OneDriveId;
                jw2.IsVisible = true;



            }

            events = new ObservableCollection<EventUser>(conn.Table<EventUser>().Where(k => k.Mine == true).ToList());

            events2 = new ObservableCollection<EventUser>(conn.Table<EventUser>().Where(k => k.Mine == false).ToList());

            events3 = new ObservableCollection<EventUser>(conn.Table<EventUser>().ToList());
            BindingContext = this;


        }

        private async void ButtonLogout_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("", "Czy chcesz się wylogować?", "Tak", "Nie");

            if (answer)
            {
                conn = DependencyService.Get<ISQLite>().GetConnection();
                List<EventUser> query = conn.Query<EventUser>("Select * From EventUser where Mine = 1"
                                 );

                foreach (EventUser x in query)
                {
                    conn.Delete(x);

                }
                events.Clear();

                conn.DeleteAll<Settings>();
                events.Clear();


                ButtonLogin.IsVisible = true;
                ButtonLogout.IsVisible = false;
                jw2.IsVisible = false;
                jw2.Text = "";
                jw2.IsVisible = false;
            }
        }

        private void ResetDatabase_Clicked(object sender, EventArgs e)
        {


            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.DeleteAll<EventUser>();
            conn.DeleteAll<EventPhoto>();
            conn.DeleteAll<Settings>();
            events.Clear();
            events2.Clear();

            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Uwaga", "Usunąłeś wszystkie dane z bazy SQLLite", "OK");
            });

            ButtonLogin.IsVisible = true;
            ButtonLogout.IsVisible = false;
            jw2.IsVisible = false;
            jw2.Text = "";
            jw2.IsVisible = false;

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

        private string komunikat;
        public string Komunikat
        {
            get
            {
                return komunikat;
            }

            set
            {
                if (komunikat != value)
                {
                    komunikat = value;
                    OnPropertyChanged("Komunikat");
                }
            }
        }
        public EventUser x1234;

        private void HandleModalPopping(object sender, ModalPoppingEventArgs e)
        {
            if (e.Modal == _myModalPage)
            {
                // now we can retrieve that phone number:
                Qrcode = _myModalPage.qrcode;
                Komunikat = _myModalPage.komunikat;
                x1234 = _myModalPage.x123;
                _myModalPage = null;

                events2.Add(x1234);

                // remember to remove the event handler:
                //  Memobook.App.Current.ModalPopping -= HandleModalPopping;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Uwaga", Komunikat, "OK");
                });

                // events2.Add(new Event() { Name = "Nowe dodane wydarzenie o nazwie...", EventId = "szczegóły wydarzenia" });
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

        public async void lvItemTapped(object sender, ItemTappedEventArgs e)
        {

            var myListView = (ListView)sender;

            var myItem = myListView.SelectedItem;


           // await Navigation.PushModalAsync(new QrCodeShow());
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


                using (var client1 = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler()))
                {

           
                    client1.DefaultRequestHeaders.Authorization
                             = new AuthenticationHeaderValue("basic", "lalala");
                    ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

                    client1.BaseAddress = new Uri(App.UrlStart + "Email/");



                    var response1 = await client1.GetAsync(userName);
                    //var response1 = RequestHelper.GetRequestAsync(requestPath1, client1).Result;
                    // JObject jObjectResponse1 = JObject.Parse(response1);

                    if (response1.IsSuccessStatusCode)
                    {
                        var str1234 = await response1.Content.ReadAsStringAsync();

                        string temp = JsonConvert.DeserializeObject(str1234) as string;
                        ButtonLogin.IsVisible = false;
                        ButtonLogout.IsVisible = true;
                        jw2.IsVisible = true;
                        jw2.Text = "Zalogowano jako " + userName;
                        jw2.IsVisible = true;

                        String[] para = temp.Split(' ');
                        BusinessId = para[0];
                        str1234 = para[1];



                        Settings s = new Settings();
                        s.OneDriveID = userName;
                        s.SecretPassword = para[1];

                        conn = DependencyService.Get<ISQLite>().GetConnection();
                        conn.CreateTable<Settings>();
                        conn.Insert(s);





                        using (var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler()))
                        {

                            //client.BaseAddress = new Uri("https://pph-ws.azurewebsites.net/Email/");
                            client.BaseAddress = new Uri(App.UrlStart + "Email/");

                            client.DefaultRequestHeaders.Authorization
                                     = new AuthenticationHeaderValue("basic", str1234);
                            ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

                            var response12 = await client.GetAsync("aa12345");

                            if (response12.IsSuccessStatusCode)
                            {
                                var str = await response12.Content.ReadAsStringAsync();
                                //udało się dodac wydarzenie nalezy je teraz wrzucić do bazy wewnętrznej.



                                List<EventUser> query = conn.Query<EventUser>("Select * From EventUser where Mine = 1"
                                    );

                                foreach (EventUser x in query)
                                {
                                    conn.Delete(x);

                                }
                                events.Clear();

                                //pobranie nowych danych do bazy po przelogowaniu


                                using (var client12 = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler()))
                                {

                                    //client12.BaseAddress = new Uri("https://pph-ws.azurewebsites.net/Event/GetAllMyEvents/");

                                    client12.BaseAddress = new Uri(App.UrlStart + "Event/GetAllMyEvents/");

                                    
                                    Encryption a = new Encryption();
                                    a.SecretEncryptor(str1234, BusinessId, out string encrypted);






                                    client12.DefaultRequestHeaders.Authorization
                                             = new AuthenticationHeaderValue("basic", encrypted);
                                    ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;


                                    var response123 = await client12.GetAsync("aa12345");

                                    if (response123.IsSuccessStatusCode)
                                    {
                                        var str1 = await response123.Content.ReadAsStringAsync();
                                        //udało się dodac wydarzenie nalezy je teraz wrzucić do bazy wewnętrznej.

                                        EventUser dodanyevent = new EventUser();
                                        //var JsonObject = JsonConvert.DeserializeObject(str);

                                        List<EventUser> UserList = JsonConvert.DeserializeObject<List<EventUser>>(str1);

                                        conn = DependencyService.Get<ISQLite>().GetConnection();
                                        conn.CreateTable<EventUser>();
                                        foreach (EventUser x in UserList)
                                        {
                                            
                                            Encryption enc = new Encryption();
                                            enc.SecretEncryptor(x.EventId, BusinessId, out string wynik);
                                            x.qrcode = wynik;
                                            //conn.Query<EventUser>("select * from EventUser",null);

                                            conn.Insert(x);
                                            events.Add(x);


                                        }
                                    }
                                }

                            }
                        }


                        //  events.Add(new EventUser() { Name = "Twoje wydarzenie nr 1", EventId = "szczegóły wydarzenia" });


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

        private async void Button_Clicked(object sender, EventArgs e)
        {

            var action = await DisplayActionSheet("Dodaj zdjęcie do wydarzenia", "Anuluj", null, "Wybierz istniejące", "Zrób zdjęcie");
            MediaFile file = null;
            if (action == "Zrób zdjęcie")
            {

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                    return;
                }
                file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg"
                });
            }
            if (action == "Wybierz istniejące")
            {

                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Photos Not Supported", "Permission not granted to photos", "OK");
 
                    return;
                }
                file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new
                                  Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full
                });
            }

            if (file == null)
                    return;

                //   await DisplayAlert("File Location", file.Path, "OK");
                byte[] aaaa;
                Stream aaaaaaaa;

                using (var memoryStream = new MemoryStream())
                {

                    file.GetStream().CopyTo(memoryStream);
                    file.Dispose();
                    aaaaaaaa = memoryStream;
                    aaaa = memoryStream.ToArray();
                }


                EventUser stringInThisCell = (EventUser)((Button)sender).BindingContext;

                conn = DependencyService.Get<ISQLite>().GetConnection();
                conn.CreateTable<EventPhoto>();
                EventPhoto x = new EventPhoto();
                x.PhotoOriginal = aaaa;
                x.DateAdded = DateTime.Now;
                x.EventUserId = stringInThisCell.EventUserID;
                x.EventId = stringInThisCell.EventId;

                conn = DependencyService.Get<ISQLite>().GetConnection();
                conn.CreateTable<EventUser>();
                EventUser a = conn.Table<EventUser>().Where(k => k.EventId == stringInThisCell.EventId).ToList()[0];


                  var x12356 = await SendRequestAsync_jw(x,a);

                //conn.Query<EventUser>("select * from EventUser",null);

                //conn.Query<EventUser>("select * from EventUser",null);

                conn.Insert(x);
        }


        public async Task<HttpResponseMessage> SendRequestAsync_jw(EventPhoto x12345, EventUser x)
        {


            using (var client12 = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler()))
            {


                // Set the Accept header for BSON.
                client12.DefaultRequestHeaders.Accept.Clear();
                client12.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                client12.BaseAddress = new Uri(App.UrlStart + "Event/AddPhoto/");
                client12.DefaultRequestHeaders.Authorization
                      = new AuthenticationHeaderValue("basic", x.SecretPassword + " " + x.qrcode);

                ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

                //  StringContent content12 = new StringContent(ImageData, Encoding.UTF8, "application/bson");
                var content = new StringContent(JsonConvert.SerializeObject(x12345), Encoding.UTF8, "application/json");
                // POST using the BSON formatter.
                //MediaTypeFormatter bsonFormatter = new BsonMediaTypeFormatter();
                var result = await client12.PostAsync("aaa", content);
                string resultContent = await result.Content.ReadAsStringAsync();
                if (result.IsSuccessStatusCode)
                {

                    conn = DependencyService.Get<ISQLite>().GetConnection();
                    conn.CreateTable<EventPhoto>();
                    x12345.EventPhotoId = Convert.ToInt32(resultContent);
                    conn.Update(x12345);
                }
            }
            return null;
        }
    


        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("", "Czy na pewno chcesz wypisać się z tego wydarzenia?", "Tak", "Nie");

            if (answer)
            {

                EventUser stringInThisCell = (EventUser)((Button)sender).BindingContext;


                conn = DependencyService.Get<ISQLite>().GetConnection();
                conn.CreateTable<EventUser>();


                //conn.Query<EventUser>("select * from EventUser",null);

                conn.Delete(stringInThisCell);
                events2.Remove(stringInThisCell);
            }

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

        
        public string ChosenEvent { get; set; }
        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            EventUser stringInThisCell = (EventUser)((Button)sender).BindingContext;

            ChosenEvent = stringInThisCell.EventId;

            ShowModalPage2();
        }



        private async void ShowModalPage2()
        {
            // When you want to show the modal page, just call this method
            // add the event handler for to listen for the modal popping event:
            Memobook.App.Current.ModalPopping += HandleModalPopping;
            _myModalPag2e = new EventInfo(ChosenEvent);
            await Navigation.PushModalAsync(_myModalPag2e);
        }
    }
}