using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChipmunkSharp
{
	public class b2Vec3
	{
		public const int b2_maxPolygonVertices = 8;

		/// Construct using coordinates.
		public b2Vec3(float ix, float iy, float iz)
		{
			x = ix;
			y = iy;
			z = iz;
		}

		/// Set this vector to all zeros.
		public void SetZero() { x = 0f; y = 0f; z = 0f; }

		/// Set this vector to some specified coordinates.
		public void Set(float ix, float iy, float iz) { x = ix; y = iy; z = iz; }

		/// Negate this vector.
		public static b2Vec3 operator -(b2Vec3 b)
		{
			b2Vec3 v = new b2Vec3(-b.x, -b.y, -b.z);
			return v;
		}

		/// Add a vector to this vector.
		public static b2Vec3 operator +(b2Vec3 v1, b2Vec3 v2)
		{
			return (new b2Vec3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z));
		}

		/// Subtract a vector from this vector.
		public static b2Vec3 operator -(b2Vec3 v1, b2Vec3 v2)
		{
			return (new b2Vec3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z));
		}

		/// Multiply this vector by a scalar.
		public static b2Vec3 operator *(b2Vec3 v1, float a)
		{
			return (new b2Vec3(v1.x * a, v1.y * a, v1.z * a));
		}

		/// Multiply this vector by a scalar.
		public static b2Vec3 operator *(float a, b2Vec3 v1)
		{
			return (new b2Vec3(v1.x * a, v1.y * a, v1.z * a));
		}


		/// Get the length squared. For performance, use this instead of
		/// b2Vec2::Length (if possible).
		public float LengthSquared()
		{
			return x * x + y * y + z * z;
		}


		/// Get the skew vector such that dot(skewvec, other) == cross(vec, other)
		public b2Vec3 Skew()
		{
			return new b2Vec3(-y, x, z);
		}

		public float x, y, z;

	}
}
