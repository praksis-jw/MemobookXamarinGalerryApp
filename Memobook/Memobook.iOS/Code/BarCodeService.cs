using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using Memobook.Interfaces;
using Memobook.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(BarcodeService))]

namespace Memobook.iOS
{
    public class BarcodeService : IBarcodeService
    {
        public Stream ConvertImageStream(string text, int width = 300, int height = 1300)
        {
            var barcodeWriter = new ZXing.Mobile.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 0
                }
            };

            barcodeWriter.Renderer = new ZXing.Mobile.BitmapRenderer();
            var bitmap = barcodeWriter.Write(text);
            var stream = bitmap.AsPNG().AsStream(); // this is the difference 
            stream.Position = 0;

            return stream;
        }
    }
}
