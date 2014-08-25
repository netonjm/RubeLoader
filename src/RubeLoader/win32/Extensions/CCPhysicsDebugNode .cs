using Box2D.Common;
using Box2D.Dynamics;
using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChipmunkSharp
{

	[Flags]
	public enum PhysicsDrawFlags
	{

		None = 1 << 0,


		/// <summary>
		/// Draw shapes.
		/// </summary>
		Shapes = 1 << 1,

		/// <summary>
		/// Draw joint connections.
		/// </summary>
		Joints = 1 << 2,

		/// <summary>
		/// Draw contact points.
		/// </summary>
		ContactPoints = 1 << 3,

		/// <summary>
		/// Draw polygon BB.
		/// </summary>
		BB = 1 << 4,

		/// <summary>
		/// Draw All connections.
		/// </summary>
		All = 1 << 10,

	}

	public class PhysicsDebugDraw : CCDrawNode
	{

		public static b2Color CONSTRAINT_COLOR = new b2Color(0, 1, 0);
		public static b2Color TRANSPARENT_COLOR = new b2Color(0, 0, 0);

		CCPoint[] springPoints = new CCPoint[]{
	new CCPoint(0.00f, 0.0f),
	new CCPoint(0.20f, 0.0f),
	new CCPoint(0.25f, 3.0f),
	new CCPoint(0.30f, -6.0f),
	new CCPoint(0.35f, 6.0f),
	new CCPoint(0.40f, -6.0f),
	new CCPoint(0.45f, 6.0f),
	new CCPoint(0.50f, -6.0f),
	new CCPoint(0.55f, 6.0f),
	new CCPoint(0.60f, -6.0f),
	new CCPoint(0.65f, 6.0f),
	new CCPoint(0.70f, -3.0f),
	new CCPoint(0.75f, 6.0f),
	new CCPoint(0.80f, 0.0f),
	new CCPoint(1.00f, 0.0f)
		};


		public PhysicsDrawFlags Flags = PhysicsDrawFlags.None;



		static CCPoint cpVert2Point(CCPoint vert)
		{
			return new CCPoint(vert.X, vert.Y);
		}


		static CCPoint[] cpVertArray2ccpArrayN(CCPoint[] cpVertArray, int count)
		{
			if (count == 0)
				return null;

			CCPoint[] pPoints = new CCPoint[count];

			for (int i = 0; i < count; ++i)
			{
				pPoints[i].X = cpVertArray[i].X;
				pPoints[i].Y = cpVertArray[i].Y;
			}
			return pPoints;
		}

#if USE_PHYSICS
		CCPhysicsWorld _world;
#endif
		b2World _space;
		b2Body _body;

		//bool ignoreBodyRotation = false;



#if USE_PHYSICS

		public PhysicsDebugDraw(CCPhysicsWorld world)
		{
			_world = world;
			_space = world.Info.Space;
			SelectFont("weblysleeku", 22);
			_world.Scene.AddChild(this); // getScene().addChild(_drawNode);

		}

#else

			public PhysicsDebugDraw(b2World space)
		{

	_space = space;

			SelectFont("weblysleeku", 22);

		}

#endif




		public b2Body Body
		{
			get
			{

				return _body;

			}
			set
			{

				_body = value;

			}
		}

		public override CCPoint Position
		{
			get
			{
				return new CCPoint(_body.Position.x,_body.Position.y);
			}

            //set
            //{
            //    //_body.p = new b2Vec2(value.X, value.Y);//();
            //}
		}



		/// <summary>
		/// Append flags to the current flags.
		/// </summary>
		public void AppendFlags(params PhysicsDrawFlags[] flags)
		{
			foreach (var item in flags)
			{
				Flags |= item;
			}

		}

		/// <summary>
		/// Clear flags from the current flags.
		/// </summary>
		public void ClearFlags(params PhysicsDrawFlags[] flags)
		{
			foreach (var item in flags)
			{
				Flags &= ~item;

			}
		}



		public void DebugDraw()
		{

			if (_space == null)
			{
				return;
			}

            //_space.BodyList
            //_space.DrawDebugData();
            DrawString(15, 15, string.Format("Bodies: {0}", _space.BodyCount));
            DrawString(15, 50, string.Format("Joins : {0}", _space.JointCount));
            DrawString(15, 80, string.Format("Contacts: {0}", _space.ContactCount));

            if (Flags.HasFlag(PhysicsDrawFlags.All) || Flags.HasFlag(PhysicsDrawFlags.BB) || Flags.HasFlag(PhysicsDrawFlags.Shapes))
            {
                for (b2Body body = _space.BodyList; body !=null ; body = body.Next)
                {
                    DrawBody()
                }
               
            }

            //if (Flags.HasFlag(PhysicsDrawFlags.Joints) || Flags.HasFlag(PhysicsDrawFlags.All))
            //{
            //    _space.EachConstraint(DrawConstraint);
            //}

            //var contacts = 0;

            //if (Flags.HasFlag(PhysicsDrawFlags.All) || Flags.HasFlag(PhysicsDrawFlags.ContactPoints))
            //{
            //    for (var i = 0; i < _space.arbiters.Count; i++)
            //    {
            //        for (int j = 0; j < _space.arbiters[i].contacts.Count; j++)
            //        {
            //            Draw(_space.arbiters[i].contacts[j]);
            //        }
            //        contacts += _space.arbiters[i].contacts.Count;
            //    }

            //}

            //DrawString(15, 110, "Contact points: " + contacts);
            //DrawString(15, 140, string.Format("Nodes:{1} Leaf:{0} Pairs:{2}", cp.numLeaves, cp.numNodes, cp.numPairs));

			base.Draw();
			base.Clear();
		}

        //public void DrawShape(cpShape shape)
        //{
        //    b2Body body = shape.body;
        //    b2Color color = new b2Color(0, 255, 0); //cp.GetShapeColor(shape); ;// ColorForBody(body);


        //    switch (shape.shapeType)
        //    {
        //        case cpShapeType.Circle:
        //            {

        //                cpCircleShape circle = (cpCircleShape)shape;

        //                if (Flags.HasFlag(PhysicsDrawFlags.BB) || Flags.HasFlag(PhysicsDrawFlags.All))
        //                    Draw(circle.bb);

        //                if (Flags.HasFlag(PhysicsDrawFlags.Shapes) || Flags.HasFlag(PhysicsDrawFlags.All))
        //                    Draw(circle, color);

        //            }
        //            break;
        //        case cpShapeType.Segment:
        //            {

        //                cpSegmentShape seg = (cpSegmentShape)shape;

        //                if (Flags.HasFlag(PhysicsDrawFlags.BB) || Flags.HasFlag(PhysicsDrawFlags.All))
        //                    Draw(seg.bb);

        //                if (Flags.HasFlag(PhysicsDrawFlags.Shapes) || Flags.HasFlag(PhysicsDrawFlags.All))
        //                {
        //                    Draw(seg, color);
        //                }



        //            }
        //            break;
        //        case cpShapeType.Polygon:
        //            {
        //                cpPolyShape poly = (cpPolyShape)shape;


        //                if (Flags.HasFlag(PhysicsDrawFlags.BB) || Flags.HasFlag(PhysicsDrawFlags.All))
        //                    Draw(poly.bb);

        //                if (Flags.HasFlag(PhysicsDrawFlags.Shapes) || Flags.HasFlag(PhysicsDrawFlags.All))
        //                {
        //                    Draw(poly, color);
        //                }


        //            }
        //            break;
        //        default:
        //            cp.AssertHard(false, "Bad assertion in DrawShape()");
        //            break;
        //    }
        //}

        //public void DrawConstraint(cpConstraint constraint)
        //{
        //    Type klass = constraint.GetType();

        //    if (klass == typeof(cpPinJoint))
        //    {
        //        Draw((cpPinJoint)constraint);
        //    }
        //    else if (klass == typeof(cpSlideJoint))
        //    {
        //        Draw((cpSlideJoint)constraint);

        //    }
        //    else if (klass == typeof(cpPivotJoint))
        //    {
        //        Draw((cpPivotJoint)constraint);
        //    }
        //    else if (klass == typeof(cpGrooveJoint))
        //    {
        //        Draw((cpGrooveJoint)constraint);
        //    }
        //    else if (klass == typeof(cpDampedSpring))
        //    {

        //        Draw((cpDampedSpring)constraint);
        //        // TODO
        //    }
        //    else if (klass == typeof(cpDampedRotarySpring))
        //    {

        //        Draw((cpDampedRotarySpring)constraint);

        //    }
        //    else if (klass == typeof(cpSimpleMotor))
        //    {

        //        Draw((cpSimpleMotor)constraint);

        //    }
        //    else
        //    {
        //        //		printf("Cannot draw constraint\n");
        //    }
        //}



		public void DrawSpring(CCPoint a, CCPoint b, b2Color b2Color)
		{

			DrawDot(a, 5, CONSTRAINT_COLOR);
			DrawDot(b, 5, CONSTRAINT_COLOR);

			CCPoint delta = CCPoint.cpvsub(b, a);
			float cos = delta.x;
			float sin = delta.y;
			float s = 1.0f / CCPoint.cpvlength(delta);

			CCPoint r1 = CCPoint.cpv(cos, -sin * s);
			CCPoint r2 = CCPoint.cpv(sin, cos * s);

			CCPoint[] verts = new CCPoint[springPoints.Length];
			for (int i = 0; i < springPoints.Length; i++)
			{
				CCPoint v = springPoints[i];
				verts[i] = new CCPoint(CCPoint.cpvdot(v, r1) + a.x, CCPoint.cpvdot(v, r2) + a.y);
			}

			for (int i = 0; i < springPoints.Length - 1; i++)
			{
				DrawSegment(verts[i], verts[i + 1], 1, b2Color.Grey);
			}

		}

		#region DRAW SHAPES

		public void DrawCircle(CCPoint center, float radius, b2Color color)
		{
			var centerPoint = center.ToCCPoint();
			var colorOutline = color.ToCCColor4B();
			var colorFill = colorOutline * 0.5f;
			base.DrawCircle(centerPoint, radius, colorOutline);
			base.DrawSolidCircle(centerPoint, radius, colorFill);
		}

		public void DrawSolidCircle(CCPoint center, float radius, b2Color color)
		{
			base.DrawCircle(center.ToCCPoint(), radius, color.ToCCColor4B());
		}

		public void DrawCircle(CCPoint center, float radius, float angle, int segments, b2Color color)
		{
			base.DrawCircle(center.ToCCPoint(), radius, angle, segments, color.ToCCColor4B());
		}
		public void DrawDot(CCPoint pos, float radius, b2Color color)
		{
			//base.DrawDot(pos.ToCCPoint(), radius, color.ToCCColor4F());
			base.DrawSolidCircle(pos.ToCCPoint(), radius, color.ToCCColor4B());
		}
		public void DrawPolygon(CCPoint[] verts, int count, b2Color fillColor, float borderWidth, b2Color borderColor)
		{
			base.DrawPolygon(cpVertArray2ccpArrayN(verts, verts.Length), count, fillColor.ToCCColor4F(), borderWidth, borderColor.ToCCColor4F());
		}
		public void DrawRect(CCRect rect, b2Color color)
		{
			base.DrawRect(rect, color.ToCCColor4B());
		}
		public void DrawSegment(CCPoint from, CCPoint to, float radius, b2Color color)
		{
			base.DrawSegment(from.ToCCPoint(), to.ToCCPoint(), radius, color.ToCCColor4F());
		}

		public void Draw(cpPolyShape poly, b2Color color)
		{
			b2Color fill = new b2Color(color);
			fill.a = cp.cpflerp(color.a, 1.0f, 0.5f);
			DrawPolygon(poly.GetVertices(), poly.Count, fill, 0.5f, color);
		}

		public void Draw(cpBB bb)
		{
			Draw(bb, b2Color.CyanBlue);
		}

		public void Draw(cpBB bb, b2Color color)
		{
			DrawPolygon(new CCPoint[] { 
 
						new CCPoint(bb.r, bb.b),
					new CCPoint(bb.r, bb.t),
					new CCPoint(bb.l, bb.t),
					new CCPoint(bb.l, bb.b)
				
				}, 4, TRANSPARENT_COLOR, 1, color);

		}

		public void Draw(cpContact contact)
		{
			DrawDot(contact.r1, 0.5f, b2Color.Red);
			DrawDot(contact.r2, 0.5f, b2Color.Red);
		}

		public void Draw(cpCircleShape circle, b2Color color)
		{
			CCPoint center = circle.tc;
			float radius = circle.r;
			CCPoint To = CCPoint.cpvadd(CCPoint.cpvmult(circle.body.GetRotation(), circle.r), (circle.tc));
			DrawCircle(center, cp.cpfmax(radius, 1.0f), color);
			DrawSegment(center, To, 0.5f, b2Color.Grey);
		}

		private void Draw(cpSegmentShape seg, b2Color color)
		{
			DrawFatSegment(seg.ta, seg.tb, seg.r, color);
		}

		private void DrawFatSegment(CCPoint ta, CCPoint tb, float r, b2Color color)
		{
			b2Color fill = new b2Color(color);
			fill.a = cp.cpflerp(color.a, 1.0f, 0.5f);

			DrawSegment(ta, tb, Math.Max(1, r), fill);
		}

		public void Draw(CCPoint point)
		{
			Draw(point, 0.5f);
		}

		public void Draw(CCPoint point, b2Color color)
		{
			Draw(point, 0.5f, color);
		}

		public void Draw(CCPoint point, float radius)
		{
			DrawDot(point, radius, b2Color.Red);
		}

		public void Draw(CCPoint point, float radius, b2Color color)
		{
			DrawDot(point, radius, color);
		}


		#endregion

		#region DRAW CONSTRAINT

		private void Draw(cpDampedRotarySpring constraint)
		{
			//Not used
		}

		private void Draw(cpDampedSpring constraint)
		{

			var a = constraint.a.LocalToWorld(constraint.GetAnchorA());
			var b = constraint.b.LocalToWorld(constraint.GetAnchorB());

			DrawSpring(a, b, CONSTRAINT_COLOR);

		}

		public void Draw(cpSimpleMotor cpSimpleMotor)
		{
			//Not used

		}

		private void Draw(cpGrooveJoint constraint)
		{

			var a = constraint.a.LocalToWorld(constraint.grv_a);
			var b = constraint.a.LocalToWorld(constraint.grv_b);
			var c = constraint.b.LocalToWorld(constraint.anchorB);

			DrawSegment(a, b, 1, CONSTRAINT_COLOR);
			DrawCircle(c, 5f, CONSTRAINT_COLOR);
		}

		private void Draw(cpPivotJoint constraint)
		{

			CCPoint a = cpTransform.Point(constraint.a.transform, constraint.GetAnchorA());
			CCPoint b = cpTransform.Point(constraint.b.transform, constraint.GetAnchorB());

			//DrawSegment(a, b, 1, b2Color.Grey);
			DrawDot(a, 3, CONSTRAINT_COLOR);
			DrawDot(b, 3, CONSTRAINT_COLOR);

		}

		public void Draw(cpSlideJoint constraint)
		{

			CCPoint a = cpTransform.Point(constraint.a.transform, constraint.GetAnchorA());
			CCPoint b = cpTransform.Point(constraint.b.transform, constraint.GetAnchorB());

			DrawSegment(a, b, 1, b2Color.Grey);
			DrawDot(a, 5, CONSTRAINT_COLOR);
			DrawDot(b, 5, CONSTRAINT_COLOR);

		}

		public void Draw(cpPinJoint constraint)
		{

			CCPoint a = cpTransform.Point(constraint.a.transform, constraint.GetAnchorA());
			CCPoint b = cpTransform.Point(constraint.b.transform, constraint.GetAnchorB());

			DrawSegment(a, b, 1, b2Color.Grey);
			DrawDot(a, 5, CONSTRAINT_COLOR);
			DrawDot(b, 5, CONSTRAINT_COLOR);


		}

		#endregion

#if USE_PHYSICS

		#region DRAW PHYSICS
		public void DrawShape(CCPhysicsShape shape)
		{
			foreach (cpShape item in shape._info.GetShapes())
				DrawShape(item);
		}

		public void DrawJoint(CCPhysicsJoint joint)
		{
			foreach (cpConstraint item in joint._info.getJoints())
				DrawConstraint(item);
		}

		#endregion

#endif

		public bool Begin()
		{
			base.Clear();
			return true;
		}

		public void End()
		{

		}


	}
}

