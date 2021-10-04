using Godot;

public class ObjectDetector : Area2D
{
	[Export]
	public NodePath CharacterPath { get; set; }
	private bool _enBody;
	public override void _Process(float delta)
	{
		var bodies = GetOverlappingBodies() ?? new Godot.Collections.Array();
		foreach (var body in GetOverlappingBodies() ?? new Godot.Collections.Array())
		{
			if (body is StaticBody2D node && node.IsInGroup("pila"))
			{
				var actionPresset = Input.IsActionPressed("ataque");
				(node as Pila).SetearVisible(actionPresset);

				if (actionPresset && Input.IsKeyPressed((int)KeyList.E))
				{
					if (CharacterPath != null)
					{
						var character = GetNode<Player>(CharacterPath);
						character.SetProcess(false);
						character.SetPhysicsProcess(false);
						character.GlobalPosition = node.GlobalPosition;
						_enBody = true;
					}
				}
			}
		}

		if (_enBody && Input.IsKeyPressed((int)KeyList.E) && !Input.IsActionPressed("ataque"))
		{
			if (CharacterPath != null)
			{
				var character = GetNode<Player>(CharacterPath);
				character.SetProcess(true);
				character.SetPhysicsProcess(true);
				_enBody = false;
			}
		}
	}
}
