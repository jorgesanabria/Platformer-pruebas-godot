using Godot;

public class DetectorRadial : Area2D
{
	public (bool, Vector2) DetectPlayer()
	{
		var bodies = GetOverlappingBodies() ?? new Godot.Collections.Array();

		foreach (var body in bodies)
		{
			if (body is Player player && !player.Cubierto)
			{
				return (true, player.Position);
			}
		}
		return (false, Vector2.Zero);
	}
}
