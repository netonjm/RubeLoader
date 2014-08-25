using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using MonoGame.Framework.WindowsPhone;
using CocosSharp;

namespace RubeLoaderTest.WP8
{
    public partial class GamePage : PhoneApplicationPage
    {
        private CCApplication sharedApp;

        // Constructor
        public GamePage()
        {
            InitializeComponent();

            this.Loaded += GamePage_Loaded;

        }

        void GamePage_Loaded(object sender, RoutedEventArgs e)
        {
            sharedApp = CCApplication.SharedApplication;
            sharedApp.ApplicationDelegate = new AppDelegate();
            CCApplication.SharedApplication.StartGame();
        }

    }
}