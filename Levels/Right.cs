using Godot;
using System;

public class Right : RayCast2D
{
	public bool RightCast()
	{
		return IsColliding();
	}
}
