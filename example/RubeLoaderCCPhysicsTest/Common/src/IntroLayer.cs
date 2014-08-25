using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CocosSharp;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ChipmunkSharp;
using System.Linq;


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
		public int DRAG_BODYS_TAG = 0x80;

		public CCNode _mouseJointNode;
		public CCPhysicsJoint _mouseJoint;

		Dictionary<int, CCNode> mouses;

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
			FILE = jsonurl;
			mouses = new Dictionary<int, CCNode>();

			//AnchorPoint = new CCPoint(0, 0);
            //Scene.Position = new CCPoint(-484.666f, 79.78197f);
			//Scale = 41;
		}

		protected override void AddedToScene()
		{
			base.AddedToScene();

			string fullpath = GetContentFileFullPath(FILE);

            //Scale changed on the Physics scene
            Scene.Scale = 23.6f;

			Console.WriteLine("Full path is: %s", fullpath);

			b2dJson json = new b2dJson();
			bool loaded = json.ReadFromFile(fullpath, this);

			if (loaded)
			{
				Console.WriteLine("Loaded JSON ok");
			}
        
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
			CCTouch touch = touches.FirstOrDefault();
			CCPoint location = touch.Location;

			List<CCPhysicsShape> shapes = Scene.PhysicsWorld.GetShapes(location);

			CCPhysicsBody body = null;

			foreach (var obj in shapes)
			{
				if ((obj.Body.Tag & DRAG_BODYS_TAG) != 0)
				{
					body = obj.Body;
					break;
				}
			}

			if (body != null)
			{
				CCNode mouse = new CCNode();

				mouse.PhysicsBody = new CCPhysicsBody();
                mouse.PhysicsBody.IsDynamic = false;
				mouse.Position = location;
				AddChild(mouse);

				CCPhysicsJointPin join = CCPhysicsJointPin.Construct(mouse.PhysicsBody, body, location);
				join.SetMaxForce(5000 * body.GetMass());
				Scene.PhysicsWorld.AddJoint(join);
				mouses.Add(touch.Id, mouse);

			}
            
		}

		public void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
		{
			var touch = touches.FirstOrDefault();
			CCNode node;
			if (mouses.TryGetValue(touch.Id, out node))
			{
				node.Position = touch.Location;
			}
		}

		public void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
		{
			var touch = touches.FirstOrDefault();

			CCNode it;
			if (mouses.TryGetValue(touch.Id, out it))
			{
				RemoveChild(it);
				mouses.Remove(touch.Id);
			}

		}



	}
}

