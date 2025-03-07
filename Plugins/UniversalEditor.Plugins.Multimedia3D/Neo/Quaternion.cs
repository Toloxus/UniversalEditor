//
//  Quaternion.cs - represents a tuple containing X, Y, Z, and W coordinates
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2013-2020 Mike Becker's Software
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace Neo
{
	public struct Quaternion : ICloneable
	{
		private double mvarX;
		private double mvarY;
		private double mvarZ;
		private double mvarW;
		public double X
		{
			get
			{
				return this.mvarX;
			}
			set
			{
				this.mvarX = value;
			}
		}
		public double Y
		{
			get
			{
				return this.mvarY;
			}
			set
			{
				this.mvarY = value;
			}
		}
		public double Z
		{
			get
			{
				return this.mvarZ;
			}
			set
			{
				this.mvarZ = value;
			}
		}
		public double W
		{
			get
			{
				return this.mvarW;
			}
			set
			{
				this.mvarW = value;
			}
		}

		public Quaternion(float x, float y, float z) : this(x, y, z, 1.0f) { }
		public Quaternion(double x, double y, double z) : this(x, y, z, 1.0) { }
		public Quaternion(float x, float y, float z, float w)
		{
			mvarX = x;
			mvarY = y;
			mvarZ = z;
			mvarW = w;
		}
		public Quaternion(double x, double y, double z, double w)
		{
			mvarX = x;
			mvarY = y;
			mvarZ = z;
			mvarW = w;
		}
		public override string ToString()
		{
			return ToString(", ", "(", ")", 4);
		}
		public string ToString(string separator, string encloseStart, string encloseEnd)
		{
			return ToString(separator, encloseStart, encloseEnd, 4);
		}
		public string ToString(string separator, string encloseStart, string encloseEnd, int maxCount)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (encloseStart != null)
			{
				sb.Append(encloseStart);
			}
			if (maxCount >= 1)
			{
				sb.Append(String.Format("{0:0.0#####################}", mvarX));
			}
			if (maxCount >= 2)
			{
				sb.Append(separator);
				sb.Append(String.Format("{0:0.0#####################}", mvarY));
			}
			if (maxCount >= 3)
			{
				sb.Append(separator);
				sb.Append(String.Format("{0:0.0#####################}", mvarZ));
			}
			if (maxCount >= 4)
			{
				sb.Append(separator);
				sb.Append(String.Format("{0:0.0#####################}", mvarW));
			}
			if (encloseEnd != null)
			{
				sb.Append(encloseEnd);
			}
			return sb.ToString();
		}

		public static Quaternion operator +(Quaternion left, Quaternion right)
		{
			return new Quaternion(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
		}
		public static Quaternion operator -(Quaternion left, Quaternion right)
		{
			return new Quaternion(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
		}
		public static Quaternion operator *(Quaternion left, Quaternion right)
		{
			return new Quaternion(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
		}
		public static Quaternion operator /(Quaternion left, Quaternion right)
		{
			return new Quaternion(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);
		}

		public double[] ToDoubleArray()
		{
			return new double[] { mvarX, mvarY, mvarZ, mvarW };
		}
		public float[] ToFloatArray()
		{
			return new float[] { (float)mvarX, (float)mvarY, (float)mvarZ, (float)mvarW };
		}

		public Matrix ToMatrix()
		{
			Matrix matrix = new Matrix(4, 4);

			double x2 = mvarX * mvarX * 2.0f;
			double y2 = mvarY * mvarY * 2.0f;
			double z2 = mvarZ * mvarZ * 2.0f;
			double xy = mvarX * mvarY * 2.0f;
			double yz = mvarY * mvarZ * 2.0f;
			double zx = mvarZ * mvarX * 2.0f;
			double xw = mvarX * mvarW * 2.0f;
			double yw = mvarY * mvarW * 2.0f;
			double zw = mvarZ * mvarW * 2.0f;

			matrix[0, 0] = 1.0f - y2 - z2;
			matrix[0, 1] = xy + zw;
			matrix[0, 2] = zx - yw;
			matrix[1, 0] = xy - zw;
			matrix[1, 1] = 1.0f - z2 - x2;
			matrix[1, 2] = yz + xw;
			matrix[2, 0] = zx + yw;
			matrix[2, 1] = yz - xw;
			matrix[2, 2] = 1.0f - x2 - y2;

			matrix[0, 3] = 0.0f;
			matrix[1, 3] = 0.0f;
			matrix[2, 3] = 0.0f;
			matrix[3, 0] = 0.0f;
			matrix[3, 1] = 0.0f;
			matrix[3, 2] = 0.0f;
			matrix[3, 3] = 1.0f;
			return matrix;
		}

		public Quaternion Slerp(Quaternion pvec4Src2, float fLerpValue)
		{
			// Slerp
			float dot = (float)((mvarX * pvec4Src2.X) + (mvarY * pvec4Src2.Y) + (mvarZ * pvec4Src2.Z) + (mvarW * pvec4Src2.W));

			// ���]����
			Quaternion vec4CorrectTarget;

			if (dot < 0.0f)
			{
				double correctTargetX = -pvec4Src2.X;
				double correctTargetY = -pvec4Src2.Y;
				double correctTargetZ = -pvec4Src2.Z;
				double correctTargetW = -pvec4Src2.W;
				vec4CorrectTarget = new Quaternion(correctTargetX, correctTargetY, correctTargetZ, correctTargetW);

				dot = -dot;
			}
			else
			{
				vec4CorrectTarget = pvec4Src2;
			}

			// �덷�΍�
			if (dot >= 1.0f) { dot = 1.0f; }
			float radian = (float)Math.Acos(dot);

			if (Math.Abs(radian) < 0.0000000001f) { return vec4CorrectTarget; }

			float inverseSin = (float)(1.0f / Math.Sin(radian));
			float t0 = (float)(Math.Sin((1.0f - fLerpValue) * radian) * inverseSin);
			float t1 = (float)(Math.Sin(fLerpValue * radian) * inverseSin);

			double x = (mvarX * t0 + vec4CorrectTarget.X * t1);
			double y = (mvarY * t0 + vec4CorrectTarget.Y * t1);
			double z = (mvarZ * t0 + vec4CorrectTarget.Z * t1);
			double w = (mvarW * t0 + vec4CorrectTarget.W * t1);
			return new Quaternion(x, y, z, w);
		}

		public Quaternion Qlerp(Quaternion pvec4Src2, float fLerpValue)
		{
			// Qlerp
			float qr = (float)(mvarX * pvec4Src2.X + mvarY * pvec4Src2.Y + mvarZ * pvec4Src2.Z + mvarW * pvec4Src2.W);
			float t0 = 1.0f - fLerpValue;

			double x, y, z, w;

			if (qr < 0)
			{
				x = mvarX * t0 - pvec4Src2.X * fLerpValue;
				y = mvarY * t0 - pvec4Src2.Y * fLerpValue;
				z = mvarZ * t0 - pvec4Src2.Z * fLerpValue;
				w = mvarW * t0 - pvec4Src2.W * fLerpValue;
			}
			else
			{
				x = mvarX * t0 + pvec4Src2.X * fLerpValue;
				y = mvarY * t0 + pvec4Src2.Y * fLerpValue;
				z = mvarZ * t0 + pvec4Src2.Z * fLerpValue;
				w = mvarW * t0 + pvec4Src2.W * fLerpValue;
			}

			Quaternion temp = new Quaternion(x, y, z, w);
			temp = temp.Normalize();
			return temp;
		}

		public Quaternion Normalize()
		{
			double x, y, z, w;
			float fSqr = (float)(1.0f / Math.Sqrt(mvarX * mvarX + mvarY * mvarY + mvarZ * mvarZ + mvarW * mvarW));

			x = mvarX * fSqr;
			y = mvarY * fSqr;
			z = mvarZ * fSqr;
			w = mvarW * fSqr;

			return new Quaternion(x, y, z, w);
		}

		public object Clone()
		{
			Quaternion clone = new Quaternion(mvarX, mvarY, mvarZ, mvarW);
			return clone;
		}
	}
}
