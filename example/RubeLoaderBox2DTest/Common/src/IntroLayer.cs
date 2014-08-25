using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;
using Box2D.Common;
using CocosSharp;
using System.Collections.Generic;
using System.Globalization;
using CocosSharp.RubeLoader;

namespace RubeLoaderTest
{

    public enum PlayerMove
    {
        MS_LEFT = 0,
        MS_STOP = 1,
        MS_RIGHT = 2,
        MS_UP = 3,
        MS_DOWN
    }


    class IntroLayer : RubeLayer
    {

        const float MIN_MILISECOND_PRESS_DELAY = 0.2f;
        CCDelayTimeEx pressDelay;

        public static string FILE = "bike.json";

        public IntroLayer(string jsonurl, CCSize size)
            : base(jsonurl,size)
        {

            
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            pressDelay = new CCDelayTimeEx(MIN_MILISECOND_PRESS_DELAY, "PressDelay");

            SetPlayerLayerName("image0");
            SetPlayerVelocity(0.7f);

            AnchorPoint = new CCPoint(0, 0);
            Position = new CCPoint(-484.666f, 79.78197f);
            Scale = 41;

#if !WINDOWS_PHONE
            InitializeKeyboardListener();
#endif
            
            Schedule();

        }


#if !WINDOWS_PHONE

        private void InitializeKeyboardListener()
        {

            CCEventListenerKeyboard keyListener = new CCEventListenerKeyboard();
            keyListener.OnKeyReleased = KeyReleased;
            AddEventListener(keyListener, this);

        }

#endif

        public override void Update(float dt)
        {
            base.Update(dt);

#if !WINDOWS_PHONE

            KeyboardState newKeyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            if (newKeyboardState.IsKeyDown(Keys.A)) // Press left to pan left.
                MoveLeft();

            if (newKeyboardState.IsKeyDown(Keys.D)) // Press right to pan right.
                MoveRight();

            if (newKeyboardState.IsKeyDown(Keys.S)) // Press down to pan down.
                MoveDown();

            if (newKeyboardState.IsKeyDown(Keys.W)) // Press up to pan up.
                MoveUp();

            if (newKeyboardState.IsKeyDown(Keys.Left) && pressDelay.HasPassed())
            {
                MoveChar(PlayerMove.MS_LEFT);
                pressDelay.Update();
            }
            if (newKeyboardState.IsKeyDown(Keys.Right) && pressDelay.HasPassed())
            {
                MoveChar(PlayerMove.MS_RIGHT);
                pressDelay.Update();
            }
            if (newKeyboardState.IsKeyDown(Keys.Up) && pressDelay.HasPassed())
            {
                MoveChar(PlayerMove.MS_UP);
                pressDelay.Update();
            }

            if (newKeyboardState.IsKeyDown(Keys.X))
            {
                float x = 0, y = 0, z = 0;
                //this->getCamera()->getCenterXYZ(&x, &y, &z);
                //this->getCamera()->setCenterXYZ(x, y + 0.0000001, z);
              //  Camera.GetCenterXyz(out x, out y, out z);
              //  Camera.SetCenterXyz(x, y + 0.0000001f, z);
            }

            if (newKeyboardState.IsKeyDown(Keys.Z))
            {
                float x = 0, y = 0, z = 0;
                //this->getCamera()->getCenterXYZ(&x, &y, &z);
                //this->getCamera()->setCenterXYZ(x, y + 0.0000001, z);
                //Camera.GetCenterXyz(out x, out y, out z);
                //Camera.SetCenterXyz(x, y - 0.0000001f, z);
            }

#endif

            Console.WriteLine("Pos ({0},{1}) / Scale: {2} / P Vel: {3} ", PositionX.ToString(new CultureInfo("en-US")), PositionY.ToString(new CultureInfo("en-US")), ScaleX, PLAYER_LAYER_VELOCITY);

        }


#if !WINDOWS_PHONE

        public void KeyReleased(CCEventKeyboard e)
        {

            if (e.Keys == CCKeys.O)
                AddGravity();

            if (e.Keys == CCKeys.P)
                RemoveGravity();

            if (e.Keys == CCKeys.T)
                PLAYER_LAYER_VELOCITY -= 0.1f;

            if (e.Keys == CCKeys.Y)
                PLAYER_LAYER_VELOCITY += 0.1f;

            if (e.Keys == CCKeys.E)
                ZoomIn();

            if (e.Keys == CCKeys.Q)
                ZoomIn();

        }

#endif

        #region ON TOUCHES OVERRIDE METHODS

        public override void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            base.OnTouchesBegan(touches, touchEvent);
        }

        public override void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            base.OnTouchesMoved(touches, touchEvent);
        }

        public override void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            base.OnTouchesEnded(touches, touchEvent);
        }

        #endregion

        #region On Mouse scroll

        public override void OnMouseScroll(CCEventMouse mouseEvent)
        {
            base.OnMouseScroll(mouseEvent);

            if (mouseEvent.ScrollY < 0)
                ZoomIn();

            if (mouseEvent.ScrollY > 0)
                ZoomOut();
        }


        #endregion
        /// <summary>
        /// Movimiento del jugador
        /// </summary>
        /// <param name="moveState"></param>
        public void MoveChar(PlayerMove moveState)
        {
            if (Player != null)
            {
                var vel = Player.Body.LinearVelocity;
                float desiredVel = 0;

                switch (moveState)
                {
                    case PlayerMove.MS_LEFT:
                        desiredVel = -PLAYER_LAYER_VELOCITY;
                        break;
                    case PlayerMove.MS_STOP:
                        desiredVel = 0;
                        break;
                    case PlayerMove.MS_RIGHT:
                        desiredVel = PLAYER_LAYER_VELOCITY;
                        break;
                    case PlayerMove.MS_UP:
                        desiredVel = PLAYER_LAYER_VELOCITY;
                        break;
                    case PlayerMove.MS_DOWN:

                        desiredVel = -PLAYER_LAYER_VELOCITY;

                        break;
                }

                float velChange = desiredVel + vel.x;
                float impulse = Player.Body.Mass * velChange; //disregard time factor


                Player.Body.ApplyLinearImpulse((moveState == PlayerMove.MS_UP || moveState == PlayerMove.MS_DOWN) ? new b2Vec2(0, impulse) : new b2Vec2(impulse, 0), Player.Body.WorldCenter);

            }

        }

        protected override void Draw()
        {
            base.Draw();
        }


    }
}

