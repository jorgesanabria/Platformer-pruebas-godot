using Godot;
using System;

public class SaltoDePared : Node, IMovementStateHandler
{
	public string HandlerName => nameof(SaltoDePared);
	[Export]
	public NodePath SensorPared { get; set; }
	private float _tiempoAcumulado = 0f;
	[Export]
	public float HorizontalForce { get; set; } = 500f;
	[Export]
	public float VerticalForce { get; set; } = 500f;
	private bool _isJumping = false;
	private bool _isOnWall;

	[Export]
	public float WallTime { get; set; } = 0.5f;
	private Vector2 _originalGravity = Vector2.Zero;
	public string Handle(CharacterController controller)
	{
		if (GetNode<RayCast2D>(SensorPared).IsColliding() && Input.IsActionPressed("ui_left") && !controller.IsOnFloor())
		{
			_originalGravity = controller.GravityVector;
			controller.GravityVector /= 3;
			//controller.Velocity = new Vector2(HorizontalForce, -VerticalForce);
			//controller.Velocity = new Vector2(controller.Velocity.x, controller.Velocity.y / 3);
			_isOnWall = true;
		}

		if (controller.IsOnFloor())
		{
			_isOnWall = false;
			controller.GravityVector = _originalGravity;
			return nameof(MoveHorizontal);
		}

		//if (_tiempoAcumulado > 0)
		//{
		//	if (Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_right"))
		//	{
		//		controller.MoveHorizontal(HorizontalDirection.Right, HorizontalForce);
		//		return HandlerName;
		//	}
		//	if (Input.IsActionPressed("ui_right") && !Input.IsActionPressed("ui_left"))
		//	{
		//		controller.MoveHorizontal(HorizontalDirection.Left, HorizontalForce);
		//		return HandlerName;
		//	}
		//}

		if (GetNode<RayCast2D>(SensorPared).IsColliding())
		{
			_tiempoAcumulado = WallTime;
		}

		_tiempoAcumulado = Mathf.Clamp(_tiempoAcumulado - controller.GetProcessDeltaTime(), 0, 10);

		return HandlerName;
	}
}
