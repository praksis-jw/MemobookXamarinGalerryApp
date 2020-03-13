using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Memobook.Services;
using Memobook.Views;
using Memobook.Data;

namespace Memobook
{
    public partial class App : Application
    {
        static EventDatabaseController eventdatabase;


        public App()
        {
            InitializeComponent();

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
