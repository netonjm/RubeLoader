using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChipmunkSharp
{
	/// A 3-by-3 matrix. Stored in column-major order.
	public struct b2Mat33
	{
		public float exx { get { return ex.x; } set { ex.x = value; } }
		public float exy { get { return ex.y; } set { ex.y = value; } }
		public float exz { get { return ex.z; } set { ex.z = value; } }

		public float eyx { get { return ey.x; } set { ey.x = value; } }
		public float eyy { get { return ey.y; } set { ey.y = value; } }
		public float eyz { get { return ey.z; } set { ey.z = value; } }

		public float ezx { get { return ez.x; } set { ez.x = value; } }
		public float ezy { get { return ez.y; } set { ez.y = value; } }
		public float ezz { get { return ez.z; } set { ez.z = value; } }

		/*
				public b2Mat33()
				{
					ex = new b2Vec3();
					ey = new b2Vec3();
					ez = new b2Vec3();
				}
		*/

		/// ruct this matrix using columns.
		public b2Mat33(b2Vec3 c1, b2Vec3 c2, b2Vec3 c3)
		{
			ex = c1;
			ey = c2;
			ez = c3;
		}

		/// Set this matrix to all zeros.
		public void SetZero()
		{
			ex.SetZero();
			ey.SetZero();
			ez.SetZero();
		}


		/// Get the inverse of this matrix as a 2-by-2.
		/// Returns the zero matrix if singular.
		public b2Mat33 GetInverse22(b2Mat33 M)
		{
			float a = ex.x, b = ey.x, c = ex.y, d = ey.y;
			float det = a * d - b * c;
			if (det != 0.0f)
			{
				det = 1.0f / det;
			}

			M.ex.x = det * d; M.ey.x = -det * b; M.ex.z = 0.0f;
			M.ex.y = -det * c; M.ey.y = det * a; M.ey.z = 0.0f;
			M.ez.x = 0.0f; M.ez.y = 0.0f; M.ez.z = 0.0f;
			return (M);
		}

	

		public b2Vec3 ex, ey, ez;
	}
}
