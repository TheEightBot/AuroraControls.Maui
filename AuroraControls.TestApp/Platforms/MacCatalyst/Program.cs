using ObjCRuntime;
using UIKit;

namespace AuroraControls.TestApp;

public static class Program
{
    // This is the main entry point of the application.
    public static void Main(string[] args)
    {
        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}