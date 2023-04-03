using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using System.IO;

namespace WGU_C971.Droid
{
    [Activity(Label = "WGU_C971", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            string fileName = "semesters_db.db3";
            string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string completepath = Path.Combine(folderPath, fileName);
            LoadApplication(new App(completepath));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            
        }
    }
}