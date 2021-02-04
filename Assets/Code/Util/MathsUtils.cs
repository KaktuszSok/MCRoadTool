using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathsUtils
{
	/// <summary>
	/// Finds the intersection between two lines each defined by a point and a direction (lines stretch infinitely)
	/// </summary>
    public static Vector2 FindLinesIntersect(Vector2 p1, Vector2 dir1, Vector2 p2, Vector2 dir2)
	{
		float c1 = dir1.y * p1.x - dir1.x * p1.y;
		float c2 = dir2.y * p2.x - dir2.x * p2.y;
		float delta = dir1.x*dir2.y - dir1.y*dir2.x;

		if (delta == 0) return new Vector2(float.NaN, float.NaN);
		return new Vector2((dir1.x * c2 - dir2.x * c1) / delta, (dir1.y * c2 - dir2.y * c1) / delta);
	}

	/// <summary>
	/// Returns anticlockwise angle between two vectors a,b in the range 0-360 degrees
	/// </summary>
	public static float DeltaAngle360(Vector2 a, Vector2 b)
	{
		float angle = Mathf.Atan2(a.x*b.y - a.y*b.x, a.x*b.x + a.y*b.y)*Mathf.Rad2Deg;
		if (float.IsNaN(angle)) return 0f;
		if (angle < 0)
			angle += 360f;
		return angle;
	}
}
