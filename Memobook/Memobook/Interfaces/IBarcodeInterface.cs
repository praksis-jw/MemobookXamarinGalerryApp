using System;
using System.IO;

namespace Memobook.Interfaces
{
    public interface IBarcodeService
    {
        Stream ConvertImageStream(string text, int width = 700, int height = 1300);
    }
}