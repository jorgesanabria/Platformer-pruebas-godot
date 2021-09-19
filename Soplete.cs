using Godot;

public class Soplete : Area2D, IEnablable
{
	[Export]
	public float DanioRealizable = 10000f;

	public override void _Ready()
	{
		foreach (var node in GetNode<Node>("particles").GetChildren() ?? new Godot.Collections.Array())
		{
			if (node is Particles2D node2d) node2d.Emitting = false;
		}
	}
	public void Enable()
	{
		foreach (var node in GetNode<Node>("particles").GetChildren() ?? new Godot.Collections.Array())
		{
			if (node is Particles2D node2d) node2d.Emitting = true;
		}
	}

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
