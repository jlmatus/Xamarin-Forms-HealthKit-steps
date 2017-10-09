using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using StepsCounterApp;
using UIKit;


namespace StepsCounterTestProject.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Auth.Presenters.XamarinIOS.AuthenticationConfiguration.Init();

			LoadApplication(new App());


            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl
            (
                UIApplication application,
                NSUrl url,
                string sourceApplication,
                NSObject annotation
            )
        {
#if DEBUG
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("OpenURL Called");
            sb.Append("     url         = ").AppendLine(url.AbsoluteUrl.ToString());
            sb.Append("     application = ").AppendLine(sourceApplication);
            sb.Append("     annotation  = ").AppendLine(annotation?.ToString());
            System.Diagnostics.Debug.WriteLine(sb.ToString());
#endif

            // Convert iOS NSUrl to C#/netxf/BCL System.Uri - common API
            Uri uri_netfx = new Uri(url.AbsoluteString);

            // load redirect_url Page
            AuthenticationState.Authenticator.OnPageLoading(uri_netfx);

            return true;
        }
    }
}
