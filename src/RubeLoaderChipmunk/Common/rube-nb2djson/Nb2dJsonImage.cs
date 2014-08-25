using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CocosSharp;


namespace ChipmunkSharp
{

	public enum _b2dJsonImagefilterType
	{
		FT_NEAREST,
		FT_LINEAR,
		FT_MAX
	};

	public class b2dJsonImage
	{
		public string Name { get; set; }
		public string File { get; set; }
		public CCPhysicsBody Body { get; set; }
		public cpVect Center { get; set; }
		public float Angle { get; set; }
		public float Scale { get; set; }
		public bool Flip { get; set; }
		public float Opacity { get; set; }

		public _b2dJsonImagefilterType Filter { get; set; }

		public cpVect[] m_corners { get; set; }

		public int NumPoints { get; set; }

		public float[] Points { get; set; }

		public float[] UvCoords { get; set; }
		public int NumIndices { get; set; }
		public short[] Indices { get; set; }

		public CCSprite Sprite { get; set; }
		public int[] ColorTint { get; set; }


		public b2dJsonImage()
		{
			Body = null;
			Center = cpVect.Zero;


			Angle = 0; Scale = 1;
			Flip = false;

			Filter = _b2dJsonImagefilterType.FT_LINEAR;

			ColorTint = new int[4];
			m_corners = new cpVect[4];

		}

		public void updateUVs(float aspect)
		{
			//set up vertices

			float hx = 0.5f * aspect;
			float hy = 0.5f;

			cpVect[] verts = new cpVect[4];
			verts[0] = new cpVect(-hx, -hy);
			verts[1] = new cpVect(hx, -hy);
			verts[2] = new cpVect(hx, hy);
			verts[3] = new cpVect(-hx, hy);

			b2Mat33 r = new b2Mat33(), s = new b2Mat33();
			_setMat33Rotation(ref r, Angle);
			_setMat33Scale(ref s, Scale, Scale);
			b2Mat33 m = _b2Mul(r, s);

			for (int i = 0; i < 4; i++)
			{
				verts[i] = _b2Mul(m, verts[i]);
				verts[i] = cpVect.cpvadd(verts[i], Center);
			}

			//set up uvs

			cpVect[] uvs = new cpVect[4];
			uvs[0] = new cpVect(0, 0);
			uvs[1] = new cpVect(1, 0);
			uvs[2] = new cpVect(1, 1);
			uvs[3] = new cpVect(0, 1);

			//set up arrays for rendering

			NumPoints = 4;
			NumIndices = 6;

			//if (points!=null) delete[] points;
			//if (uvCoords) delete[] uvCoords;
			//if (indices) delete[] indices;

			Points = new float[2 * NumPoints];
			UvCoords = new float[2 * NumPoints];
			Indices = new short[NumIndices];

			for (int i = 0; i < NumPoints; i++)
			{
				Points[2 * i + 0] = verts[i].x;
				Points[2 * i + 1] = verts[i].y;
				UvCoords[2 * i + 0] = uvs[i].x;
				UvCoords[2 * i + 1] = uvs[i].y;
			}

			Indices[0] = 0;
			Indices[1] = 1;
			Indices[2] = 2;
			Indices[3] = 2;
			Indices[4] = 3;
			Indices[5] = 0;

		}



		public float RenderOrder { get; set; }



		void _setMat33Translation(ref b2Mat33 mat, cpVect t)
		{
			mat.SetZero();
			mat.ex.x = 1;
			mat.ey.y = 1;
			mat.ez.x = t.x;
			mat.ez.y = t.y;
			mat.ez.z = 1;
		}
		void _setMat33Rotation(ref b2Mat33 mat, float angle)
		{
			mat.SetZero();
			float c = cp.cpfcos(angle), s = cp.cpfsin(angle);
			mat.ex.x = c; mat.ey.x = -s;
			mat.ex.y = s; mat.ey.y = c;
			mat.ez.z = 1;
		}

		void _setMat33Scale(ref b2Mat33 mat, float xfactor, float yfactor)
		{
			mat.SetZero();
			mat.ex.x = xfactor;
			mat.ey.y = yfactor;
			mat.ez.z = 1;
		}

		cpVect _b2Mul(b2Mat33 A, cpVect v2)
		{
			b2Vec3 v = new b2Vec3(v2.x, v2.y, 1);
			b2Vec3 r = v.x * A.ex + v.y * A.ey + v.z * A.ez;
			return new cpVect(r.x, r.y);
		}

		b2Mat33 _b2Mul(b2Mat33 B, b2Mat33 A)
		{
			return new b2Mat33(b2Mul(A, B.ex), b2Mul(A, B.ey), b2Mul(A, B.ez));
		}
		/// Multiply a matrix times a vector.
		public b2Vec3 b2Mul(b2Mat33 A, b2Vec3 v)
		{
			return v.x * A.ex + v.y * A.ey + v.z * A.ez;
		}


	}
}

