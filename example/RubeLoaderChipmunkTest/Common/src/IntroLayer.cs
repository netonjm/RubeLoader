using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CocosSharp;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ChipmunkSharp;



namespace RubeLoaderCCPhysicsTest
{

	public enum PlayerMove
	{
		MS_LEFT = 0,
		MS_STOP = 1,
		MS_RIGHT = 2,
		MS_UP = 3,
		MS_DOWN
	}


	class IntroLayer : CCLayer
	{

		CCEventListenerKeyboard kListener;
		CCEventListenerTouchAllAtOnce tListener;

		const float MIN_MILISECOND_PRESS_DELAY = 0.2f;
		//CCDelayTimeEx pressDelay;

		public static string FILE = "bike.json";

		public static string GetContentDirectoryFullPath()
		{
			return Path.GetFullPath("Content");
		}

		public static string GetContentFileFullPath(string file)
		{
			return Path.Combine(GetContentDirectoryFullPath(), file);
		}

	
		public IntroLayer(string jsonurl, CCSize size):base(size)
		{

			//AnchorPoint = new CCPoint(0, 0);
            //Scene.Position = new CCPoint(-484.666f, 79.78197f);
			//Scale = 41;
		}

		protected override void AddedToScene()
		{
			base.AddedToScene();

            
            
        
            // Scale = .3f;

			string fullpath = GetContentFileFullPath(FILE);

			Console.WriteLine("Full path is: %s", fullpath);

			b2dJson json = new b2dJson();
			bool loaded = json.ReadFromFile(fullpath, this);

			if (loaded)
			{
				Console.WriteLine("Loaded JSON ok");
                Scene.PhysicsWorld.Gravity = new CCPoint(0, 0);
				//b2BodyDef bodyDef = new b2BodyDef();

				//m_touch = new MouseTouch(m_world, this);

				//m_touch.m_mouseJointGroundBody = m_world.CreateBody(bodyDef);

				//AfterLoadProcessing(json);
			}

            Scene.Scale = 10.6f;

			tListener = new CCEventListenerTouchAllAtOnce();
			tListener.OnTouchesBegan = OnTouchesBegan;
			tListener.OnTouchesEnded = OnTouchesEnded;
			tListener.OnTouchesMoved = OnTouchesMoved;
            Scene.Position = Window.WindowSizeInPixels.Center;

			AddEventListener(tListener, this);

			Schedule();

		}


		public override void OnExit()
		{
			base.OnExit();
			RemoveEventListener(kListener);
			RemoveEventListener(tListener);
		}


		public override void Update(float dt)
		{
			base.Update(dt);

            Scene.Update(dt);

		}


		public void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
		{
			//base.OnTouchesBegan(touches, touchEvent);
            CCMouse.Instance.OnTouchesBegan(touches, Scene);
            
		}

		public void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
		{
			// base.OnTouchesMoved(touches, touchEvent);

            CCMouse.Instance.OnTouchesMoved(touches, Scene);
		}

		public void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
		{
			//base.OnTouchesEnded(touches, touchEvent);
            CCMouse.Instance.OnTouchesEnded(touches, Scene);

		}



	}
}

