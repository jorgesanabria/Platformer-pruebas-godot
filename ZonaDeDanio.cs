using Godot;
using System;

public class ZonaDeDanio : Area2D
{
	[Export]
	public float DanioRealizable { get; set; }
	public override void _Process(float delta)
	{
		foreach (var body in GetOverlappingBodies() ?? new Godot.Collections.Array())
		{
			if (body is Player player)
			{
				player.vida -= delta * DanioRealizable;
				GD.Print(player.vida);
			}
		}
	}
}
