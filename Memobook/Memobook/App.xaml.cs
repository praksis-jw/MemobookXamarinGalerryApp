using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Memobook.Services;
using Memobook.Views;
using Memobook.Data;
using DLToolkit.Forms.Controls;

namespace Memobook
{
    public partial class App : Application
    {
        static EventDatabaseController eventdatabase;
        public static string UrlStart { get; set; }

        public App()
        {
            InitializeComponent();
            FlowListView.Init();
            Plugin.Iconize.Iconize.With(new Plugin.Iconize.Fonts.FontAwesomeRegularModule());
            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static EventDatabaseController EventDatabase
        {
            get 
            {
                if (eventdatabase == null)
                    eventdatabase = new EventDatabaseController();
                return eventdatabase;
            }
        }
    }
}
