using Godot;
using System;

public class Boton : Sprite
{
	[Export]
	public NodePath Izquierda { get; set; }
	[Export]
	public NodePath Derecha { get; set; }
	[Export]
	public NodePath Arriba { get; set; }
	[Export]
	public NodePath Abajo { get; set; }
	[Export]
	public bool SoyActual { get; set; }

	private static bool KeyReleased = true;

	private Player _player;

	public override void _Process(float delta)
	{
		if (SoyActual && KeyReleased)
		{
			GetNode<Label>("Label").Text = "Actual";
			if (Input.IsActionJustPressed("Izquierda") && Izquierda != null)
			{
				KeyReleased = false;
				MarcarNodo(Izquierda);
			}
			else if (Input.IsActionJustPressed("Derecha") && Derecha != null)
			{
				KeyReleased = false;
				MarcarNodo(Derecha);
			}
			else if (Input.IsActionJustPressed("Arriba") && Arriba != null)
			{
				KeyReleased = false;
				MarcarNodo(Arriba);
			}
			else if (Input.IsActionJustPressed("Abajo") && Abajo != null)
			{
				KeyReleased = false;
				MarcarNodo(Abajo);
			}
		}
		else
		{
			GetNode<Label>("Label").Text = string.Empty;
		}
		if (!Input.IsActionPressed("Izquierda") && !Input.IsActionPressed("Derecha") && !Input.IsActionPressed("Arriba") && !Input.IsActionPressed("Abajo"))
		{
			KeyReleased = true;
		}
	}

	private void MarcarNodo(NodePath path)
	{
		if (path == null) return;
		var node = GetNode<Boton>(path);
		if (node == null) return;
		SoyActual = false;
		node.SoyActual = true;
	}
}
