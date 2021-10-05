using Godot;
using System.Collections.Generic;
using System.Linq;

public class ObjectDetector : Area2D
{
	[Export]
	public NodePath CharacterPath { get; set; }
	private bool _enBody;
	public override void _Process(float delta)
	{
		var botones = new List<Boton>();
		if (Input.IsKeyPressed((int)KeyList.Q))
		{
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

			if (Input.IsKeyPressed((int)KeyList.E))
			{
				var selected = botones.First(x => x.SoyActual);
				if (CharacterPath != null)
				{
					var character = GetNode<Player>(CharacterPath);
					character.SetProcess(false);
					character.SetPhysicsProcess(false);
					character.GlobalPosition = selected.GlobalPosition;
				}
			}
		}

		foreach (var body in GetOverlappingBodies() ?? new Godot.Collections.Array())
		{
			if (body is Pila pila && pila.IsInGroup("pila"))
			{
				botones.Add(pila.GetNode<Boton>("Boton"));
			}
		}

		if (botones.Any(x => x.SoyActual) && Input.IsKeyPressed((int)KeyList.E) && !Input.IsKeyPressed((int)KeyList.Q))
		{
			if (CharacterPath != null)
			{
				var character = GetNode<Player>(CharacterPath);
				character.SetProcess(true);
				character.SetPhysicsProcess(true);
				botones.Select(x => x.SoyActual = false).ToList();
			}
		}


		//var first = true;
		//var bodies = GetOverlappingBodies() ?? new Godot.Collections.Array();
		//foreach (var body in GetOverlappingBodies() ?? new Godot.Collections.Array())
		//{
		//	if (body is StaticBody2D node && node.IsInGroup("pila"))
		//	{
		//		var actionPresset = Input.IsActionPressed("ataque");
		//		(node as Pila).SetearVisible(actionPresset);

		//		if (actionPresset)
		//		{
		//			if (first)
		//			{
		//				var boton = node.GetNode<Boton>("Boton");
		//				boton.SoyActual = true;
		//				first = false;
		//			}
		//		}

		//		if (actionPresset && Input.IsKeyPressed((int)KeyList.E))
		//		{
		//			if (CharacterPath != null)
		//			{
		//				var character = GetNode<Player>(CharacterPath);
		//				character.SetProcess(false);
		//				character.SetPhysicsProcess(false);
		//				character.GlobalPosition = node.GlobalPosition;
		//				_enBody = true;
		//			}
		//		}
		//	}
		//}

		//if (_enBody && Input.IsKeyPressed((int)KeyList.E) && !Input.IsActionPressed("ataque"))
		//{
		//	if (CharacterPath != null)
		//	{
		//		var character = GetNode<Player>(CharacterPath);
		//		character.SetProcess(true);
		//		character.SetPhysicsProcess(true);
		//		_enBody = false;
		//	}
		//}
	}
}
