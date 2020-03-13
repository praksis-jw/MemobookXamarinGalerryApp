using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Memobook.Droid;
using Memobook.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(BarcodeService))]

namespace Memobook.Droid
{
    public class BarcodeService : IBarcodeService
    {
        public Stream ConvertImageStream(string text, int width = 300, int height = 700)
        {
            var barcodeWriter = new ZXing.Mobile.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin =0
                }
            };

            barcodeWriter.Renderer = new ZXing.Mobile.BitmapRenderer();
            var bitmap = barcodeWriter.Write(text);
            var stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);  // this is the diff between iOS and Android
            stream.Position = 0;
            return stream;
        }
    }
}