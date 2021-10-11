using Godot;
using System;

public class A : Sprite
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		var moved = false;
		var lastPosition = GlobalPosition;
		if (Input.IsActionPressed("ui_right"))
		{
			var pos = Position;
			pos.x += 100 * delta;
			Position = pos;
			moved = true;
		}

		if (Input.IsActionPressed("ui_left"))
		{
			var pos = Position;
			pos.x -= 100 * delta;
			Position = pos;
			moved = true;
		}

		var bPosition = GetNode<Sprite>("B").GlobalPosition;

		if (moved)
		{
			bPosition.x = lastPosition.x - 10;
			GetNode<Sprite>("B").GlobalPosition = bPosition;
		}
	}
}
