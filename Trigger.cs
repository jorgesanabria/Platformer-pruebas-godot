using Godot;
using System.Collections.Generic;

public class Trigger : Area2D
{
	[Export]
	public NodePath PathToNodes { get; set; }
	private List<IEnablable> _Enablables { get; set; } = new List<IEnablable>();
	public override void _Ready()
	{
		foreach (var toEnable in (GetNode<Node>(PathToNodes)?.GetChildren() ?? new Godot.Collections.Array()))
		{
			if (toEnable is IEnablable en)
			{
				_Enablables.Add(en);
			}
		}
	}
	public override void _Process(float delta)
	{
		foreach (var node in (GetOverlappingBodies() ?? new Godot.Collections.Array()))
		{
			if (node is Player)
			{
				foreach (var toEnable in _Enablables)
				{
					toEnable.Enable();
				}
				break;
			}
		}
	}
}
public interface IEnablable
{
	void Enable();
}
