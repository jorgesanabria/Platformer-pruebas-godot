using Godot;
using System;

public class Left : RayCast2D
{
	public bool LeftCast()
	{
		return IsColliding();
	}
}
