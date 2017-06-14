using UIKit;

namespace Satori.Rtm.Test
{
    public class Application
    {
        // This is the main entry point of the application.
        private static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "TestAppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "TestAppDelegate");
        }
    }
}