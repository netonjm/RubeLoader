using System.Reflection;
using Microsoft.Xna.Framework;
using CocosDenshion;
using CocosSharp;

namespace RubeLoaderTest.WP8
{
    public class AppDelegate : CCApplicationDelegate
    {
        int preferredWidth;
        int preferredHeight;

        /// <summary>
        ///  Implement CCDirector and CCScene init code here.
        /// </summary>
        /// <returns>
        ///  true  Initialize success, app continue.
        ///  false Initialize failed, app terminate.
        /// </returns>
        public override void ApplicationDidFinishLaunching(CCApplication application)
        {


            //1280 x 768
#if WINDOWS_PHONE
            preferredWidth = 1280;
            preferredHeight = 768;
#else
            preferredWidth = 1280;
            preferredHeight = 768;
#endif

            application.PreferredBackBufferWidth = preferredWidth;
            application.PreferredBackBufferHeight = preferredHeight;

            application.PreferMultiSampling = true;
            application.ContentRootDirectory = "Content";

            CCDirector director = CCApplication.SharedApplication.MainWindowDirector;
            director.DisplayStats = true;
            director.AnimationInterval = 1.0 / 60;

            CCSize designSize = new CCSize(preferredWidth, preferredHeight);

            if (CCDrawManager.FrameSize.Height > preferredHeight)
            {
                CCSize resourceSize = new CCSize(preferredWidth, preferredHeight);
                //CCSize resourceSize = new CCSize(preferredWidth, preferredHeight);
                application.ContentSearchPaths.Add("hd");
                director.ContentScaleFactor = resourceSize.Height / designSize.Height;
            }

            CCDrawManager.SetDesignResolutionSize(designSize.Width, designSize.Height, CCResolutionPolicy.ShowAll);

            // turn on display FPS
            director.DisplayStats = true;

            // set FPS. the default value is 1.0/60 if you don't call this
            director.AnimationInterval = 1.0 / 60;

            CCScene pScene = IntroLayer.Scene;

            director.RunWithScene(pScene);
        }



    }
}