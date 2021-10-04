using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Botones : Node2D
{
	private List<Node2D> _botones = new List<Node2D>();
	public override void _Ready()
	{
		//foreach (var child in GetChildren()) _botones.Add(child as Node2D);

		//_botones.First().GetNode<Label>("Label").Text = "Actual";
	}

	public override void _Process(float delta)
	{

	}
}
