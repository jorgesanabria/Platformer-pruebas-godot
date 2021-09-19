using Godot;
using System;

public class CajaDeObjeto : Area2D
{
	private Node2D _child;
	public override void _Ready()
	{
		_child = GetNode<Node>("Objeto").GetChild(0) as Node2D;
	}

	public override void _Process(float delta)
	{
		if (_child == null)
			return;

		foreach (var body in GetOverlappingBodies() ?? new Godot.Collections.Array())
		{
			if (body is Player player)
			{
				if (Input.IsActionPressed("ui_accept"))
				{
					var inventario = player.GetNode<Node2D>("inventario/objetos");
					if (inventario != null)
					{
						if (_child != null)
						{
							_child.GetParent().RemoveChild(_child);
							inventario.AddChild(_child);
							//_child.Transform = inventario.Transform;
							_child.Position = inventario.Position;
						}
					}
				}
			}
		}
	}
}
