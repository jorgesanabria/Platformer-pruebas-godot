using Godot;
using System;

public class MovimientoParabolico1 : Sprite
{
	[Export]
	public NodePath position2;
	private Node2D _position2;
	public override void _Ready()
	{
		_position2 = GetNode<Node2D>(position2);
	}

	float time_pass = 0.0f;
	const float COMPLETE_TIME = 5.0f;

	public override void _Process(float delta)
	{
		if (Input.IsActionPressed("ui_right"))
		{
			if (Position.x <= _position2.Position.x)
			{
				var pos = Position;
				pos.x += 100 * delta;
				pos.y -= 1;
				Position = pos;
			}
			else
			{

				var pos = Position;
				pos.x += 100 * delta;
				pos.y += 1;
				Position = pos;
			}
		}
		if (Input.IsActionPressed("ui_left"))
		{
			if (Position.x <= _position2.Position.x)
			{
				var pos = Position;
				pos.x -= 100 * delta;
				pos.y += 1;
				Position = pos;
			}
			else
			{

				var pos = Position;
				pos.x -= 100 * delta;
				pos.y -= 1;
				Position = pos;
			}
		}


	}
}
