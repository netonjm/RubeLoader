using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text;
using System.IO;
using Box2D.Dynamics;
using Box2D.Common;
using CocosSharp;


namespace CocosSharp.RubeLoader
{
    public class Nb2dJsonImage
    {
        public string Name { get; set; }
        public string File { get; set; }

        public CCSprite Sprite { get; set; }

        public b2Body Body { get; set; }
        public b2Vec2 Center { get; set; }
        public float Angle { get; set; }
        public float Scale { get; set; }
        public bool Flip { get; set; }
        public float Opacity { get; set; }
        public int Filter { get; set; }
        public float RenderOrder { get; set; }
        public int[] ColorTint { get; set; }
        public b2Vec2[] Corners { get; set; }
        public int NumPoints { get; set; }
        public float[] Points { get; set; }
        public float[] UvCoords { get; set; }
        public int NumIndices { get; set; }
        public short[] Indices { get; set; }

        public b2Fixture[] fixture = null;
        //internal b2Body body;

        public Nb2dJsonImage()
        {
            ColorTint = new int[4];
        }

    }
}

