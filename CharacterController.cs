using Godot;
using System.Collections.Generic;
using System.Linq;

public class CharacterController : KinematicBody2D
{
	[Export]
	public NodePath MovementStates { get; set; }
	[Export]
	public Vector2 GravityVector { get; set; } = new Vector2(0, 98);
	public Vector2 FloorNormal { get; set; } = new Vector2(0, -1);
	[Export]
	public float Speed { get; set; } = 100;
	[Export]
	public HorizontalDirection Direction { get; set; } = HorizontalDirection.Right;
	[Export]
	public string CurrentState { get; set; }
	private List<IMovementStateHandler> _StateHandlers { get; set; } = new List<IMovementStateHandler>();
	public Vector2 Velocity { get; set; }
	private bool _ScaleHorizontal;
	public override void _Ready()
	{
		foreach (var node in GetNode<Node>(MovementStates)?.GetChildren() ?? new Godot.Collections.Array())
		{
			if (node is IMovementStateHandler handler) _StateHandlers.Add(handler);
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!IsOnFloor())
		{
			Velocity += GravityVector;
		}

		foreach (var stateHandler in _StateHandlers.Where(x => x.HandlerName == CurrentState))
		{
			var nextState = stateHandler.Handle(this);
			if (nextState != CurrentState)
			{
				CurrentState = nextState;
				break;
			}
			
		}

		Velocity = MoveAndSlide(Velocity, FloorNormal);
	}

	public void MoveHorizontal(HorizontalDirection direction, float plus = 0)
	{
		switch (direction)
		{
			case HorizontalDirection.None:
				Velocity = new Vector2(0, Velocity.y);
				break;
			case HorizontalDirection.Right:
				var transform1 = Transform;
				if (_ScaleHorizontal)
				{
					transform1.x *= -1;
					Transform = transform1;
					_ScaleHorizontal = false;
				}
				Velocity = new Vector2(Speed + plus, Velocity.y);
				break;
			case HorizontalDirection.Left:
				var transform2 = Transform;
				if (!_ScaleHorizontal)
				{
					transform2.x *= -1;
					Transform = transform2;
					_ScaleHorizontal = true;
				}
				Velocity = new Vector2(-(Speed + plus), Velocity.y);
				break;
		}
	}
}

public interface IMovementStateHandler
{
	string HandlerName { get; }
	string Handle(CharacterController controller);
}
public enum HorizontalDirection
{
	None,
	Right,
	Left
}
