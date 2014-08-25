using Box2D.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box2D.Common
{
    public class Settings
    {

        public b2Vec2 viewCenter = new b2Vec2(0.0f, 20.0f);
        public float hz = 60.0f;
        public int velocityIterations = 8;
        public int positionIterations = 3;
        public bool drawShapes = true;
        public bool drawJoints = true;
        public bool drawAABBs = false;
        public bool drawPairs = false;
        public bool drawContactPoints = false;
        public int drawContactNormals = 0;
        public int drawContactForces = 0;
        public int drawFrictionForces = 0;
        public bool drawCOMs = false;
        public bool drawStats = true;
        public bool drawProfile = true;
        public int enableWarmStarting = 1;
        public int enableContinuous = 1;
        public int enableSubStepping = 0;
        public bool pause = false;
        public bool singleStep = false;
    }

}
