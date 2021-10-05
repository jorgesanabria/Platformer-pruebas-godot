using Godot;
using System.Collections.Generic;
using System.Linq;

public class ObjectDetector : Area2D
{
	[Export]
	public NodePath CharacterPath { get; set; }
	public override void _Process(float delta)
	{
		var botones = new List<Boton>();
		foreach (var body in GetOverlappingBodies() ?? new Godot.Collections.Array())
		{
			if (body is Pila pila && pila.IsInGroup("pila"))
			{
				botones.Add(pila.GetNode<Boton>("Boton"));
			}
		}

		if (!botones.Any(x => x.SoyActual))
		{
			var d = botones.FirstOrDefault();
			if (d != null) d.SoyActual = true;
		}

		if (Input.IsActionJustPressed("Accion"))
		{
			var selected = botones.First(x => x.SoyActual);
			if (CharacterPath != null)
			{
				var player = GetNode<Player>(CharacterPath);
				if (!player.IsProcessing() && !player.IsPhysicsProcessing() && player.GlobalPosition == selected.GlobalPosition)
				{
					player.SetProcess(true);
					player.SetPhysicsProcess(true);
				}
				else
				{
					player.SetProcess(false);
					player.SetPhysicsProcess(false);
					player.GlobalPosition = selected.GlobalPosition;
				}
			}
		}
	}
}
