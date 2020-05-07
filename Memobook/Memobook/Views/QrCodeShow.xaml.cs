using Memobook.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Memobook.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QrCodeShow : ContentPage
    {
        public QrCodeShow( string text)
        {
            InitializeComponent();

            var stream = DependencyService.Get<IBarcodeService>().ConvertImageStream(text);
            barcode.Source= ImageSource.FromStream(() => { return stream; });
        }
    }
}