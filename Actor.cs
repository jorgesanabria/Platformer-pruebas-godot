using Godot;
using System.Collections.Generic;
using System.Linq;

public class Actor : KinematicBody2D
{
	[Export]
	public NodePath StateHandlersPath { get; set; }
	[Export]
	public float MovementVelocity = 10;
	[Export]
	public Vector2 GravityVector = new Vector2(0f, 9.8f);
	[Export]
	public Vector2 FloorNormal = new Vector2(0f, -1f);
	[Export]
	public string CurrentState;

	protected virtual List<IStateHandler> _StateHandlers { get; set; } = new List<IStateHandler>();
	private Vector2 _MoveDirection = Vector2.Zero;
	private Vector2 _Velocity = Vector2.Zero;

	public void MoveTo(WayPoint point)
	{
		var normal = (point.Position - Position).Normalized() * MovementVelocity;
		_Velocity = new Vector2(normal.x, _Velocity.y);
	}
	public void MoveTo(Vector2 point)
	{
		var normal = (point- Position).Normalized() * MovementVelocity;
		_Velocity = new Vector2(normal.x, _Velocity.y);
	}

	public override void _Ready()
	{
		foreach (var handler in GetNode(StateHandlersPath)?.GetChildren() ?? new Godot.Collections.Array())
		{
			if (handler is IStateHandler validStateHandler) _StateHandlers.Add(validStateHandler);
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!IsOnFloor())
		{
			_Velocity += GravityVector;
		}
		_Velocity = MoveAndSlide(_Velocity, FloorNormal);

		var toHandle = _StateHandlers.Where(x => x.StateToHandle == CurrentState);

		foreach (var handler in toHandle)
		{
			var result = handler.HandleState(this);
			if (result != CurrentState)
			{
				CurrentState = result;
				break;
			}
		}
	}
}

public interface IStateHandler
{
	string StateToHandle { get; }
	string HandleState(Actor actor);
}
