using System.Reflection;
using CocosDenshion;
using CocosSharp;

namespace RubeLoaderCCPhysicsTest
{
    public class AppDelegate : CCApplicationDelegate
    {

		public static CCWindow SharedWindow { get; set; }

		public static CCSize DefaultResolution;


		/// <summary>
		///  Implement CCDirector and CCScene init code here.
		/// </summary>
		/// <returns>
		///  true  Initialize success, app continue.
		///  false Initialize failed, app terminate.
		/// </returns>
		public override void ApplicationDidFinishLaunching(CCApplication application, CCWindow mainWindow)
		{

			SharedWindow = mainWindow;

			DefaultResolution = new CCSize(
				application.MainWindow.WindowSizeInPixels.Width,
				application.MainWindow.WindowSizeInPixels.Height);

			application.ContentRootDirectory = "Content";
			application.ContentSearchPaths.Add("SD");

			CCScene scene = new CCScene(mainWindow,true);
			CCLayer layer = new IntroLayer("bike.json",DefaultResolution);
			scene.AddChild(layer);

            layer.Scene.PhysicsWorld.DebugDrawMask = CCPhysicsWorld.DEBUGDRAW_SHAPE;

			mainWindow.RunWithScene(scene);
		}



    }
}