using System;
using System.Collections.Generic;
using System.Linq;
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
        public string qrcode { get; set; }
        public BarcodeScanner()
        {
            InitializeComponent();
     
        }

        public async void Handle_OnScanResult(Result result)
        {
            qrcode = result.Text;
            //DisplayAlert("Scanned result", result.Text, "OK");
            await Navigation.PopModalAsync();
            //Device.BeginInvokeOnMainThread(async () =>
            //{
            //    await DisplayAlert("Scanned result", result.Text, "OK");
            //    await Navigation.PopModalAsync();
            //});
        }
    }
}