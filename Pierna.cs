using Godot;

public class Pierna : Position2D
{
	[Export]
	public float Offset { get; set; }
	[Export]
	public float Speed { get; set; } = 500f;

	private PathFollow2D _follower;
	private OneJointIK _oneJointIK;
	public override void _Ready()
	{
		_follower = GetNode<PathFollow2D>("Path2D/PathFollow2D");
		_oneJointIK = GetNode<OneJointIK>("OneJointIK");
		_follower.Offset = Offset;
	}
	public override void _Process(float delta)
	{
		_follower.Offset += delta * Speed;
		_oneJointIK.ReachToward(_follower.GlobalPosition);
	}
}
