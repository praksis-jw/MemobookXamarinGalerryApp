using System;
using System.Collections.Generic;
using System.Text;

namespace Memobook
{
    class AppCustomSettings
    {
        // this is the default static instance you'd use like string text = Settings.Default.SomeSetting;
        public readonly static AppCustomSettings Default = new AppCustomSettings();

        public string UrlStart { get; set; } // add setting properties as you wish

    }
}
