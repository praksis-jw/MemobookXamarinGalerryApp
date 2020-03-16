﻿using System;
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

//=c50[MnHBA44/NbWe.Ms6?lo8f2t63kg

namespace Memobook.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainWindow : ContentPage
    {
        public ImageSource BarcodeImageSource { get; set; }
        public MainWindow()
        {
            InitializeComponent();
             QRButton.Clicked += QRButton_Clicked;
           
            }
        private async void QRButton_Clicked(object sender, EventArgs e)
        {

            var authenticator = new OAuth2Authenticator(
        clientId: "251af922-5cae-4dc9-99c7-052d02158d99",
        scope: "https://graph.microsoft.com/User.Read",
        authorizeUrl: new Uri("https://login.microsoftonline.com/common/oauth2/V2.0/authorize"), // the auth URL for the service
        redirectUrl: new Uri("https://graph.microsoft.com/v1.0/me")); // the redirect URL for the service

         


            //var authenticator = new OAuth2Authenticator(
            //"251af922-5cae-4dc9-99c7-052d02158d99",
            //"HTMg=MAM=Zc8XW31F@Yqm8wcnzzQATx/",
            //"https://graph.microsoft.com/User.Read",
            //new Uri("https://login.microsoftonline.com/common/oauth2/v2.0/authorize"),
            //  new Uri("https://graph.microsoft.com/v1.0/me"),
            //new Uri("https://login.microsoftonline.com/common/oauth2/v2.0/token"),
            //null,
            //true);


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
                
                string userName = jObjectResponse["userPrincipalName"].Value<string>();
                await DisplayAlert(userName, userName, userName);
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


     

        //public async Task<object> GetPlacesAsync()
        //{
        //    //var client = new System.Net.Http.HttpClient();
        //    //client.BaseAddress = new Uri("https://pph-ws.azurewebsites.net/api/");

        //    //var response = await client.GetAsync("Glossary");
        //    //return 1;




        //    string urlParams = "{ \"html\": \"html\", \"width \": \"2\", \"height  \": \"2\", \"ratio  \": \"2\" }";
        //    var r = RequestHelper.PostRequestStringAsync("https://pph-ws.azurewebsites.net/Glossary/test12", urlParams, "application/json", new System.Net.Http.HttpClient()).Result;

        //    var s = 1;

        //    return 1;

        //}

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