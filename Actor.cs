using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Actor<TState> : KinematicBody2D
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
	public TState CurrentState;

	private List<IStateHandler<Actor<TState>>> _StateHandlers { get; set; } = new List<IStateHandler<Actor<TState>>>();
	private Vector2 _MoveDirection = Vector2.Zero;
	private Vector2 _Velocity = Vector2.Zero;

	protected void MoveTo(WayPoint point)
	{
		_MoveDirection = (Position - point.Position).Normalized();
	}

	public override void _Ready()
	{
		var handlers = GetNode(StateHandlersPath)?.GetChildren() ?? new Godot.Collections.Array();
		foreach (var handler in handlers) _StateHandlers.Add((IStateHandler<Actor<TState>>)handler);
	}
	public override void _PhysicsProcess(float delta)
	{
		if (!IsOnFloor())
		{
			_Velocity += GravityVector;
		}
		_Velocity = MoveAndSlide(_Velocity * _MoveDirection * MovementVelocity, FloorNormal);

		var toHandle = _StateHandlers.Where(x => CompareState(x.StateToHandle));

		foreach (var handler in toHandle)
        {
			var result = handler.HandleState(this);
			if (!CompareState(result))
            {
				CurrentState = result;
				break;
            }
		}
	}

	protected virtual bool CompareState(TState toCompare)
    {
		return true;
    }

	public interface IStateHandler<TActor> where TActor : Actor<TState>
	{
		TState StateToHandle { get; }
		TState HandleState(TActor actor);
	}
}
