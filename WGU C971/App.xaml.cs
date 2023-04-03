using System;
using WGU_C971.Services;
using WGU_C971.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGU_C971
{
    public partial class App : Application
    {
        public static string FilePath;
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }

        public App(string filePath)
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());

            FilePath = filePath;
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
    }
}
