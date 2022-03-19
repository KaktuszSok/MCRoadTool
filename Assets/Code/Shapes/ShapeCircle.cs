using UnityEngine;

namespace Code.Shapes
{
	public class ShapeCircle : RoadShape
	{
		/// <summary>
		/// A point on the circle's circumference which will determine its radius
		/// </summary>
		protected Vector2Int centrePoint;
		protected float radius { get; private set; }

		protected bool alternateMode = false;

		public override string GetName()
		{
			return "Circle";
		}

		public override void OnKeyHeld()
		{
			centrePoint = PlayerInput.mousePos;
			radius = (startPoint - centrePoint).magnitude;

			if (Input.GetMouseButton(1))
				alternateMode = true;
			else
				alternateMode = false;

			if(alternateMode)
			{
				centrePoint = startPoint;
			}

			base.OnKeyHeld();
		}
		public override float CalculateCellDistance(int x, int y)
		{
			float dx = x - centrePoint.x;
			float dy = y - centrePoint.y;
			return Mathf.Abs(Mathf.Sqrt(dx*dx + dy*dy) - radius); //distance from centreline of road
		}

		public override Vector2Int GetNewStartPoint()
		{
			if (alternateMode)
				return centrePoint;
			else
				return startPoint;
		}
	}
}
