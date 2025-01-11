using System;

namespace VRCApi
{
	// Token: 0x02000042 RID: 66
	internal static class Mathfrep
	{
		// Token: 0x06000168 RID: 360 RVA: 0x0000A5A8 File Offset: 0x000087A8
		public static float Abs(float value)
		{
			return (value < 0f) ? (0f - value) : value;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000A5CC File Offset: 0x000087CC
		public static float Sqrt(float value)
		{
			return (float)Math.Sqrt((double)value);
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000A5E8 File Offset: 0x000087E8
		public static float Pow(float baseValue, float exponent)
		{
			return (float)Math.Pow((double)baseValue, (double)exponent);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000A604 File Offset: 0x00008804
		public static float Cos(float radians)
		{
			return (float)Math.Cos((double)radians);
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000A620 File Offset: 0x00008820
		public static float Sin(float radians)
		{
			return (float)Math.Sin((double)radians);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000A63C File Offset: 0x0000883C
		public static float Tan(float radians)
		{
			return (float)Math.Tan((double)radians);
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000A658 File Offset: 0x00008858
		public static float Acos(float value)
		{
			return (float)Math.Acos((double)value);
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000A674 File Offset: 0x00008874
		public static float Asin(float value)
		{
			return (float)Math.Asin((double)value);
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000A690 File Offset: 0x00008890
		public static float Atan(float value)
		{
			return (float)Math.Atan((double)value);
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000A6AC File Offset: 0x000088AC
		public static float Atan2(float y, float x)
		{
			return (float)Math.Atan2((double)y, (double)x);
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000A6C8 File Offset: 0x000088C8
		public static float Clamp(float value, float min, float max)
		{
			bool flag = value < min;
			float num;
			if (flag)
			{
				num = min;
			}
			else
			{
				bool flag2 = value > max;
				if (flag2)
				{
					num = max;
				}
				else
				{
					num = value;
				}
			}
			return num;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000A6F8 File Offset: 0x000088F8
		public static float Min(float a, float b)
		{
			return (a < b) ? a : b;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000A714 File Offset: 0x00008914
		public static float Max(float a, float b)
		{
			return (a > b) ? a : b;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000A730 File Offset: 0x00008930
		public static float Lerp(float a, float b, float t)
		{
			t = Mathfrep.Clamp(t, 0f, 1f);
			return a + (b - a) * t;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000A75C File Offset: 0x0000895C
		public static float MoveTowards(float current, float target, float maxDelta)
		{
			bool flag = Mathfrep.Abs(target - current) <= maxDelta;
			float num;
			if (flag)
			{
				num = target;
			}
			else
			{
				num = current + Mathfrep.Clamp(target - current, 0f - maxDelta, maxDelta);
			}
			return num;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000A798 File Offset: 0x00008998
		public static float Repeat(float value, float length)
		{
			return value - (float)Math.Floor((double)(value / length)) * length;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000A7B8 File Offset: 0x000089B8
		public static float SmoothStep(float from, float to, float t)
		{
			t = Mathfrep.Clamp(t, 0f, 1f);
			t = t * t * (3f - 2f * t);
			return from + (to - from) * t;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000A7F8 File Offset: 0x000089F8
		public static float RadiansToDegrees(float radians)
		{
			return radians * 57.295776f;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000A814 File Offset: 0x00008A14
		public static float DegreesToRadians(float degrees)
		{
			return degrees * 0.017453292f;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000A830 File Offset: 0x00008A30
		public static float NormalizeAngle(float angle)
		{
			while (angle < 0f)
			{
				angle += 360f;
			}
			while (angle >= 360f)
			{
				angle -= 360f;
			}
			return angle;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000A878 File Offset: 0x00008A78
		public static int Sign(float value)
		{
			return (value < 0f) ? (-1) : ((value > 0f) ? 1 : 0);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000A8A4 File Offset: 0x00008AA4
		public static int RoundToInt(float value)
		{
			return (int)Math.Round((double)value);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000A8C0 File Offset: 0x00008AC0
		public static float RandomRange(float min, float max)
		{
			Random random = new Random();
			return (float)((double)min + random.NextDouble() * (double)(max - min));
		}

		// Token: 0x040000FE RID: 254
		public const float PI = 3.1415927f;

		// Token: 0x040000FF RID: 255
		public const float Deg2Rad = 0.017453292f;

		// Token: 0x04000100 RID: 256
		public const float Rad2Deg = 57.295776f;

		// Token: 0x04000101 RID: 257
		public const float Epsilon = 1E-45f;
	}
}
