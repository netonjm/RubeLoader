using Box2D.Collision;
using CocosSharp;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box2D.Common
{
    class Box2dDebugDraw : b2Draw
    {

        static int DEBUG_DRAW_MAX_VERTICES = 64;
        static int DEBUG_DRAW_CIRCLE_SEGMENTS = 16;
        float mRatio = 0;

        CCPoint[] mVertices = new CCPoint[DEBUG_DRAW_MAX_VERTICES];

        public Box2dDebugDraw(int aRatio)
            : base(aRatio)
        {

        }


        public override void DrawPolygon(b2Vec2[] aVertices, int aVertexCount, b2Color aColor)
        {
            for (int i = 0; i < DEBUG_DRAW_MAX_VERTICES && i < aVertexCount; i++)
                mVertices[i] = new CCPoint(mRatio * aVertices[i].x, mRatio * aVertices[i].y);

            CCPoint[] ar = new CCPoint[aVertices.Length];
            for (int i = 0; i < aVertices.Length; i++)
            {
                ar[i] = new CCPoint(aVertices[i].x, aVertices[i].y);
            }

            CCDrawingPrimitives.Begin();
            CCDrawingPrimitives.DrawPoly(ar, aVertexCount, true, new CCColor4B(aColor.r, aColor.g, aColor.b, 1));
            CCDrawingPrimitives.End();
        }


        public override void DrawSolidPolygon(b2Vec2[] aVertices, int aVertexCount, b2Color aColor)
        {
            for (int i = 0; i < DEBUG_DRAW_MAX_VERTICES && i < aVertexCount; i++)
                mVertices[i] = new CCPoint(mRatio * aVertices[i].x, mRatio * aVertices[i].y);

            CCPoint[] ar = new CCPoint[aVertices.Length];
            for (int i = 0; i < aVertices.Length; i++)
            {
                ar[i] = new CCPoint(aVertices[i].x, aVertices[i].y);
            }

            CCDrawManager.BlendFunc(new CCBlendFunc(CCOGLES.GL_SRC_ALPHA, CCOGLES.GL_ONE_MINUS_SRC_ALPHA));
            //glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

            CCDrawingPrimitives.Begin();
            CCDrawingPrimitives.DrawSolidPoly(mVertices, aVertexCount, new CCColor4B(aColor.r, aColor.g, aColor.b, 0.5f));
            CCDrawingPrimitives.DrawPoly(ar, aVertexCount, true, new CCColor4B(aColor.r, aColor.g, aColor.b, 1));
            CCDrawingPrimitives.End();
        }


        public override void DrawCircle(b2Vec2 aCenter, float aRadius, b2Color aColor)
        {

            CCDrawingPrimitives.Begin();

            CCDrawingPrimitives.DrawCircle(new CCPoint(mRatio * aCenter.x, mRatio * aCenter.y),
                aRadius, 0, DEBUG_DRAW_CIRCLE_SEGMENTS, false,
                new CCColor4B(aColor.r, aColor.g, aColor.b, 1));

            CCDrawingPrimitives.End();
        }


        public override void DrawSolidCircle(b2Vec2 aCenter, float aRadius, b2Vec2 aAxis,
                b2Color aColor)
        {

            float coef = 2.0f * (float)Math.PI / DEBUG_DRAW_CIRCLE_SEGMENTS;

            mVertices[0] = new CCPoint(mRatio * aCenter.x, mRatio * aCenter.y);

            for (int i = 0; i <= DEBUG_DRAW_CIRCLE_SEGMENTS; i++)
            {
                float rads = i * coef;

                float j = aRadius * (float)Math.Cos(rads) + mRatio * aCenter.x;
                float k = aRadius * (float)Math.Sin(rads) + mRatio * aCenter.y;

                mVertices[i] = new CCPoint(j, k);
            }

            CCPoint p = new CCPoint(aCenter.x + aRadius * aAxis.x, aCenter.y + aRadius * aAxis.y);
            CCDrawingPrimitives.Begin();
            CCDrawingPrimitives.DrawSolidPoly(mVertices, DEBUG_DRAW_CIRCLE_SEGMENTS, new CCColor4B(aColor.r, aColor.g, aColor.b, 0.5f));
            CCDrawingPrimitives.DrawCircle(new CCPoint(mRatio * aCenter.x, mRatio * aCenter.y), aRadius, 0, DEBUG_DRAW_CIRCLE_SEGMENTS, false, new CCColor4B(aColor.r, aColor.g, aColor.b, 1));
            CCDrawingPrimitives.DrawLine(new CCPoint(aCenter.x, aCenter.y), new CCPoint(p.X, p.Y), new CCColor4B(aColor.r, aColor.g, aColor.b, 1));
            //ccDrawLine(ccp(aCenter.X, aCenter.Y), ccp());

            CCDrawingPrimitives.End();
        }


        public override void DrawSegment(b2Vec2 aP1, b2Vec2 aP2, b2Color aColor)
        {

            CCDrawingPrimitives.Begin();
            CCDrawingPrimitives.DrawLine(new CCPoint(aP1.x, aP1.y), new CCPoint(aP2.x, aP2.y), new CCColor4B(aColor.r, aColor.g, aColor.b, 1));
            CCDrawingPrimitives.End();
        }


        public override void DrawTransform(b2Transform aXf)
        {
            b2Vec2 p1 = aXf.p, p2;
            float k_axisScale = 0.4f;

            p2 = p1 + k_axisScale * aXf.q.GetXAxis();

            DrawSegment(p1, p2, new b2Color(1, 0, 0));

            p2 = p1 + k_axisScale * aXf.q.GetYAxis();
            DrawSegment(p1, p2, new b2Color(0, 1, 0));

        }


        public void DrawPoint(b2Vec2 aP, float aSize, b2Color aColor)
        {

            CCDrawingPrimitives.Begin();

            CCDrawingPrimitives.DrawPoint(new CCPoint(mRatio * aP.x, mRatio * aP.y), 1, new CCColor4B(aColor.r, aColor.g, aColor.b, 1));

            CCDrawingPrimitives.End();
        }


        public void DrawString(int aX, int aY, string aString)
        {
            //string font = "MarkerFelt-22.xnb";
            float fontSize = 22;
            // float loadedSize;

            CCTexture2D font = new CCTexture2D(aString, "MarkerFelt", fontSize);



            if (font == null)
            {
                CCLog.Log("Failed to load default font. No font supported.");
                return;
            }

            //float scale = 1f;

            //if (loadedSize != 0)
            //{
            //    scale = fontSize / loadedSize * CCSpriteFontCache.FontScale;
            //}

            //if (dimensions.Equals(CCSize.Zero))
            //{
            //    CCVector2 temp = font.MeasureString(text).ToCCVector2();
            //    dimensions.Width = temp.X * scale;
            //    dimensions.Height = temp.Y * scale;
            //}

            // CCTexture2D tmp = new CCTexture2D(font.Texture);
            CCDrawManager.BeginDraw();
            CCDrawManager.BindTexture(font);
            CCDrawManager.EndDraw();

        }


        public void DrawAABB(b2AABB aAabb, b2Color aColor)
        {
            mVertices[0] = new CCPoint(aAabb.LowerBound.x * mRatio, aAabb.LowerBound.y * mRatio);
            mVertices[1] = new CCPoint(aAabb.UpperBound.x * mRatio, aAabb.LowerBound.y * mRatio);
            mVertices[2] = new CCPoint(aAabb.UpperBound.x * mRatio, aAabb.UpperBound.y * mRatio);
            mVertices[3] = new CCPoint(aAabb.LowerBound.x * mRatio, aAabb.UpperBound.y * mRatio);

            CCDrawingPrimitives.Begin();
            CCDrawingPrimitives.DrawPoly(mVertices, 8, true, new CCColor4B(aColor.r, aColor.g, aColor.b, 1));
            CCDrawingPrimitives.End();
        }


        public void Begin()
        {
            CCDrawingPrimitives.Begin();
        }

        public void End()
        {
            CCDrawingPrimitives.End();
        }
    }
}
