using Godot;
using System;

public class Covertura : Node2D
{
	public override void _Process(float delta)
	{
		var bodies = GetNode<Area2D>("Area2D")?.GetOverlappingBodies() ?? new Godot.Collections.Array();

		foreach (var body in bodies)
		{
			if (body is Player)
			{
				var player = body as Player;
				if (Input.IsActionPressed("covertura"))
				{
					player.Cubierto = true;
					GD.Print("Cuvierto");
				}
				else
				{
					player.Cubierto = false;
				}
				break;
			}
		}
	}
}
