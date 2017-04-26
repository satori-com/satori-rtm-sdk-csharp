using System.Reflection;
using Foundation;
using MonoTouch.NUnit.UI;
using NUnit.Framework;
using Satori.Common;
using Satori.Rtm.Client;
using UIKit;

namespace Satori.Rtm.Test
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("UnitTestAppDelegate")]
    public partial class UnitTestAppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        private UIWindow _window;
        private TouchRunner _runner;

        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            UnhandledExceptionWatcher.OnError += exn =>
            {
                Assert.Fail("Unhandled exception in event handler: " + exn.Message);
            };

            DefaultLoggers.Dispatcher.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.Serialization.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.Connection.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.Client.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.ClientRtm.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.ClientRtmSubscription.SetLevel(Logger.LogLevel.Verbose);

            RtmConfig.Init();

            // create a new window instance based on the screen size
            _window = new UIWindow(UIScreen.MainScreen.Bounds);
            _runner = new TouchRunner(_window);

            // register every tests included in the main application/assembly
            _runner.Add(Assembly.GetExecutingAssembly());

            _window.RootViewController = new UINavigationController(_runner.GetViewController());

            // make the window visible
            _window.MakeKeyAndVisible();

            return true;
        }
    }
}