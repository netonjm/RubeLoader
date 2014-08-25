using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Dynamics.Contacts;
using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.RubeLoader
{
    public class PlanetCuteRubeLayer : RubeLayer
    {
        // Override superclass to load different RUBE scene

        //b2Body m_playerBody;                                       // duh...
        //b2Fixture m_footSensorFixture;                             // a small sensor fixture attached to the bottom of the player body to detect when it's standing on something
        int m_numFootContacts;                                      // the current number of other fixtures touching the foot sensor. If this is > 0 the character is standing on something
        Nb2dJsonImage m_instructionsSprite1 = null;
        Nb2dJsonImage m_instructionsSprite2 = null;
        Nb2dJsonImage m_instructionsSprite3 = null;
        List<PlanetCuteFixtureUserData> m_allPickups = null;          // a array containing every pickup in the scene. This is used to loop through every tick and make them wobble around.
        List<PlanetCuteFixtureUserData> m_pickupsToProcess = null;    // The contact listener will put some pickups in this set when the player touches them. After the Step has finished, the
        //       layer will look in this list and process any pickups (remove them from world, play sound, count score etc).
        //       This is made a set instead of an array because a set prevents duplicate objects from being added. Sometimes
        //       a pickup can collide with more than one other fixture in the same time step (eg. the player and the foot sensor)
        //       and looping through an array with duplicates would result in removing the same body from the scene twice -> crash.


        public PlanetCuteRubeLayer(string jsonfile)
            : base(jsonfile)
        {

        }

        // Override superclass to set different starting offset
        public override CCPoint InitialWorldOffset()
        {
            //place (0,0) of physics world at center of bottom edge of screen
            CCSize s = Director.WindowSizeInPixels;
            return new CCPoint(s.Width / 2, 0);
        }


        // Override superclass to set different starting scale
        public override float InitialWorldScale()
        {
            CCSize s = Director.WindowSizeInPixels;
            return s.Height / 8; //screen will be 8 physics units high
        }

        // This is called after the Box2D world has been loaded, and while the b2dJson information
        // is still available to do extra loading. Here is where we obtain the named items in the scene.
        public override void AfterLoadProcessing(Nb2dJson json)
        {
            // call superclass method to load images etc
            base.AfterLoadProcessing(json);

            // preload the sound effects
            //SimpleAudioEngine::sharedEngine()->preloadEffect("jump.wav");
            //SimpleAudioEngine::sharedEngine()->preloadEffect("pickupgem.wav");
            //SimpleAudioEngine::sharedEngine()->preloadEffect("pickupstar.wav");

            // find player body and foot sensor fixture
            m_playerBody = json.GetBodyByName("player");
            m_footSensorFixture = json.GetFixtureByName("footsensor");

            // find all fixtures in the scene named 'pickup' and loop over them
            List<b2Fixture> pickupFixtures;
            pickupFixtures = json.GetFixturesByName("pickup").ToList();

            foreach (var f in pickupFixtures)
            {
                //For every pickup fixture, we create a FixtureUserData to set in
                //the user data.
                PlanetCuteFixtureUserData fud = new PlanetCuteFixtureUserData();
                m_allPickups.Add(fud);
                f.UserData = fud;

                // set some basic properties of the FixtureUserData
                fud.fixtureType = _fixtureType.FT_PICKUP;
                fud.body = f.Body;
                fud.originalPosition = f.Body.Position;

                // use the custom properties given to the fixture in the RUBE scene
                //fud.pickupType = (_pickupType)json.GetCustomInt(f, "pickuptype", PT_GEM);
                //json.Equals
                //fud.bounceSpeedH = json.GetCustomFloat(f, "horizontalbouncespeed");
                //fud.bounceSpeedV = json.GetCustomFloat(f, "verticalbouncespeed");
                //fud.bounceWidth  = json.GetCustomFloat(f, "bouncewidth");
                //fud.bounceHeight = json.GetCustomFloat(f, "bounceheight");

                //these "bounce deltas" are just a number given to sin when wobbling
                //the pickups. Each pickup has its own value to stop them from looking
                //like they are moving in unison.
                fud.bounceDeltaH = CCRandom.Float_0_1() * (float)Math.PI;
                fud.bounceDeltaV = CCRandom.Float_0_1() * (float)Math.PI;
            }



            // find the imageInfos for the text instruction images. Sprites 2 and 3 are
            // hidden initially
            m_instructionsSprite1 = null;
            m_instructionsSprite2 = null;
            m_instructionsSprite2 = null;


            foreach (var imgInfo in m_imageInfos)
            {
                if (imgInfo.Name == "instructions1")
                    m_instructionsSprite1 = imgInfo;
                if (imgInfo.Name == "instructions2")
                {
                    m_instructionsSprite2 = imgInfo;
                    m_instructionsSprite2.Sprite.Opacity = 0; // hide
                }
                if (imgInfo.Name == "instructions3")
                {
                    m_instructionsSprite3 = imgInfo;
                    m_instructionsSprite3.Sprite.Opacity = 0; // hide
                }
            }

            // Create a contact listener and let the Box2D world know about it.
            m_contactListener = new PlanetCuteContactListener();
            m_world.SetContactListener(m_contactListener);

            // Give the listener a reference to this class, to use in the callback
            m_contactListener.m_layer = this;

            // set the movement control touches to nil initially
            //m_leftTouch = null;
            //m_rightTouch = null;

            // initialize the values for ground detection
            m_numFootContacts = 0;
            m_jumpTimeout = 0;

            // camera will start at body position
            m_cameraCenter = m_playerBody.Position;
        }


        // This method should undo anything that was done by afterLoadProcessing, and make sure
        // to call the superclass method so it can do the same
        public override void Clear()
        {
            base.Clear();
            //SimpleAudioEngine::sharedEngine()->unloadEffect("jump.wav");
            //SimpleAudioEngine::sharedEngine()->unloadEffect("pickupgem.wav");
            //SimpleAudioEngine::sharedEngine()->unloadEffect("pickupstar.wav");

            m_playerBody = null;
            m_footSensorFixture = null;

            //delete m_contactListener;

            m_allPickups.Clear();

            //RUBELayer::clear();
        }


        public Box2D.Dynamics.b2Body m_playerBody { get; set; }

        public Box2D.Dynamics.b2Fixture m_footSensorFixture { get; set; }

        enum _fixtureType
        {
            FT_PLAYER = 0,
            FT_PICKUP = 1
        };

        enum _pickupType
        {
            PT_GEM = 0,
            PT_STAR = 1
        };

        struct PlanetCuteFixtureUserData
        {
            public _fixtureType fixtureType;
            public _pickupType pickupType;
            public b2Body body;

            public b2Vec2 originalPosition;
            public float bounceDeltaH;
            public float bounceDeltaV;
            public float bounceSpeedH;
            public float bounceSpeedV;
            public float bounceWidth;
            public float bounceHeight;
        };

        class PlanetCuteContactListener : b2ContactListener
        {

            public PlanetCuteRubeLayer m_layer;

            public override void PreSolve(b2Contact contact, Box2D.Collision.b2Manifold oldManifold)
            {

            }

            public override void PostSolve(b2Contact contact, ref b2ContactImpulse impulse)
            {

            }

            public override void BeginContact(b2Contact contact)
            {
                base.BeginContact(contact);

                PlanetCuteRubeLayer layer = m_layer;
                b2Fixture fA = contact.GetFixtureA();
                b2Fixture fB = contact.GetFixtureB();

                if (fA == layer.m_footSensorFixture || fB == layer.m_footSensorFixture)
                    layer.m_numFootContacts++;
                //CCLOG("Num foot contacts: %d", layer->m_numFootContacts);

                PlanetCuteFixtureUserData fudA = (PlanetCuteFixtureUserData)fA.UserData;
                PlanetCuteFixtureUserData fudB = (PlanetCuteFixtureUserData)fB.UserData;

                if (fudA.fixtureType == _fixtureType.FT_PICKUP && fB.Body == layer.m_playerBody)
                    layer.m_pickupsToProcess.Add(fudA);
                if (fudB.fixtureType == _fixtureType.FT_PICKUP && fA.Body == layer.m_playerBody)
                    layer.m_pickupsToProcess.Add(fudB);
            }   // called by Box2D during the Step function when two fixtures begin touching

            // called by Box2D during the Step function when two fixtures finish touching
            public override void EndContact(b2Contact contact)
            {
                base.EndContact(contact);

                PlanetCuteRubeLayer layer = (PlanetCuteRubeLayer)m_layer;
                b2Fixture fA = contact.GetFixtureA();
                b2Fixture fB = contact.GetFixtureB();

                if (fA == layer.m_footSensorFixture || fB == layer.m_footSensorFixture)
                    layer.m_numFootContacts--;
                //CCLOG("Num foot contacts: %d", layer->m_numFootContacts);
            }

        };


        public int m_jumpTimeout { get; set; }

        public b2Vec2 m_cameraCenter { get; set; }

        private PlanetCuteContactListener m_contactListener { get; set; }
    }
}
