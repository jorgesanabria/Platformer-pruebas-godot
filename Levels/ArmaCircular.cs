using Godot;

public class ArmaCircular : Area2D
{
	public void Atacar()
	{
		foreach (var node in GetOverlappingBodies() ?? new Godot.Collections.Array())
		{
			if (node is IDamagable damagable)
			{
				damagable.Damage();
			}
		}
	}
}
