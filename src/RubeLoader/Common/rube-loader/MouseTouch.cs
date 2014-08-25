using Box2D.Collision;
using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Dynamics.Joints;
using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.RubeLoader
{
    public class MouseTouch
    {

        public b2World m_world;
        public b2Vec2 m_mouseWorld;
        public b2MouseJoint m_mouseJoint;

        public b2Body m_groundBody;

        public b2Body m_mouseJointGroundBody;
        public CCTouch m_mouseJointTouch { get; set; }

        public RubeBasicLayer parent;

        bool IsArrastring = false;

        public CCPoint originPosition;

        public MouseTouch(b2World world, RubeBasicLayer layer)
        {
            parent = layer;
            m_world = world;
            b2BodyDef bodyDef = new b2BodyDef();
            m_groundBody = m_world.CreateBody(bodyDef);

        }

        public virtual void MouseUp(b2Vec2 p)
        {

            if (IsArrastring)
            {
                //parent.Position = null;
                IsArrastring = false;
            }
            else
            {
                if (m_mouseJoint != null)
                {
                    m_world.DestroyJoint(m_mouseJoint);
                    m_mouseJoint = null;
                }
            }
        }

        public void MouseMove(b2Vec2 p)
        {
            m_mouseWorld = p;

            //Si está en modo arrastre
            if (IsArrastring)
            {

                CCPoint translation = new CCPoint(p.x, p.y) - originPosition;
                CCPoint newPos = parent.Position + translation;
                parent.Position = newPos;

            }
            else
            {
                if (m_mouseJoint != null)
                {
                    m_mouseJoint.SetTarget(p);
                }
            }

        }

        public virtual bool MouseDown(b2Vec2 p)
        {
            m_mouseWorld = p;

            if (m_mouseJoint != null)
            {
                return false;
            }

            // Make a small box.
            b2AABB aabb = new b2AABB();
            b2Vec2 d = new b2Vec2();
            d.Set(0.001f, 0.001f);
            aabb.LowerBound = p - d;
            aabb.UpperBound = p + d;

            // Query the world for overlapping shapes.
            QueryCallback callback = new QueryCallback(p);
            m_world.QueryAABB(callback, aabb);

            if (callback.m_fixture != null)
            {

                b2Body body = callback.m_fixture.Body;
                b2MouseJointDef md = new b2MouseJointDef();
                md.BodyA = m_groundBody;
                md.BodyB = body;
                md.target = p;
                md.maxForce = 1000.0f * body.Mass;
                m_mouseJoint = (b2MouseJoint)m_world.CreateJoint(md);
                body.SetAwake(true);
                return true;

            }
            else
            {

                //Como no ha encontrado un objeto empezamos el arrastre      
                IsArrastring = true;
                originPosition = new CCPoint(p.x, p.y);
                return true;

            }

        }

        public void clear()
        {
            m_mouseJoint = null;
            m_mouseJointGroundBody = null;
        }
    }
}
