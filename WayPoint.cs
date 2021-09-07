using Godot;
using System.Collections.Generic;

public class WayPoint : Position2D
{
	public WayPoint NextWayPoint(List<WayPoint> points)
	{
		var index = points.IndexOf(this);
		return index == -1 || points.Count - 1 == index ? points[0] : points[index + 1];
	}
}
