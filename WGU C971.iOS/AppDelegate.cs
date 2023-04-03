using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Foundation;
using UIKit;

namespace WGU_C971.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            // Ask the user for permission to show notifications on iOS 10.0+ at startup.
            // If not asked at startup, user will be asked when showing the first notification.
            Plugin.LocalNotification.NotificationCenter.AskPermission();

            string fileName = "semesters_db.db3";
            string folderPath = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "..", "Library");
            string completepath = Path.Combine(folderPath, fileName);

            LoadApplication(new App(completepath));

            return base.FinishedLaunching(app, options);
        }

        public override void WillEnterForeground(UIApplication uiApplication)
        {
            base.WillEnterForeground(uiApplication);

            Plugin.LocalNotification.NotificationCenter.ResetApplicationIconBadgeNumber(uiApplication);
        }
    }
}