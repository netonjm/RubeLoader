
using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubePlatformTest
{
    class Player : CCSprite
    {
        public Player()
        {
            CCSpriteSheet tmp = new CCSpriteSheet("poring.plist");

            //List<CCSpriteFrame> elementos = new List<CCSpriteFrame>();
            //for (int i = 1; i <= 3; i++)
            //{
            //    tmp = CCSpriteFrameCache.SharedSpriteFrameCache.SpriteFrameByName(String.Format("walk{0}.png", i));
            //    elementos.Add(tmp);
            //}
            CCAnimation anim = new CCAnimation(tmp.Frames, 0.5f);
            CCRepeatForever rep = new CCRepeatForever(new CCAnimate(anim));
            RunAction(rep);

        }
    }
}
