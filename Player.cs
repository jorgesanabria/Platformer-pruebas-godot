using Godot;
using MaquinaDeEstados;
using MaquinaDeEstados.FSM;
using MaquinaDeEstados.InputHandler;
using System;
using System.Collections.Generic;

public class Player : KinematicBody2D
{
	public bool Cubierto = false;
	[Export]
	public Vector2 GravityVector = new Vector2(0f, 9.8f);
	[Export]
	public Vector2 GlobalVelocity = Vector2.Zero;
	[Export]
	public Vector2 FloorNormal = new Vector2(0f, -1f);
	[Export]
	public float HorizontalSpeed = 50f;
	[Export]
	public float JumpForce = 100f;
	[Export]
	public float WallJumpHorizontalForce = 50f;
	[Export]
	public float JumpFromWallHorizontalForce = 300f;
	private float _wallTime = 0f;
	[Export]
	public float WallTime = 0.5f;
	private float _dashTime = 0f;
	[Export]
	public float DashTime = 0.5f;
	[Export]
	public float DashFactor = 4;
	private bool _dashed = false;
	public bool _atacando = false;
	protected FiniteStateMachine<PlayerState, Player> _fsm;
	protected InputHandler<InputActions, string> _input;
	[Export]
	public NodePath CollisionWeaponNode;
	public float vida = 10000f;
	private bool _ScaleHorizontal;

	private Vector2 _beforePosition = Vector2.Zero;
	public override void _Ready()
	{
		_beforePosition = Position;
		GetNode<Kemono>("Slot/Kemono").Rotation = 0;
		_input = new InputHandler<InputActions, string>(
			map: new Dictionary<InputActions, string>
			{
				{ InputActions.MoveLeft, "ui_left" },
				{ InputActions.MoveRight, "ui_right" },
				{ InputActions.Jump, "ui_up" },
				{ InputActions.Attack, "ui_accept" },
				{ InputActions.Dash, "dash" }
			},
			actionJustPressed: Input.IsActionJustPressed,
			actionPressed: Input.IsActionPressed,
			actionReleased: Input.IsActionJustReleased
		);
		_fsm = new FiniteStateMachine<PlayerState, Player>(equalizer: (current, captured) => current == captured) { InitialState = PlayerState.OnAir };
		_fsm.Add(PlayerState.OnAir, (current, player) =>
		{
			if (!IsOnFloor() && !IsOnWall())
			{
				GlobalVelocity += GravityVector;
				return PlayerState.OnAir;
			}
			return IsOnFloor() && !IsOnWall() ? PlayerState.OnGround : current;
		});
		_fsm.Add(PlayerState.OnGround, (current, player) =>
		{
			if (_input.IsActionPressed(InputActions.Jump))
			{
				var jump = new Vector2(GlobalVelocity.x, -JumpForce);
				GlobalVelocity = jump;
				GetNode<Kemono>("Slot/Kemono").Saltar();
				return PlayerState.OnAir;
			}
			return current;
		});
		_fsm.Add(PlayerState.OnGround, (current, player) =>
		{
			if (_input.IsActionPressed(InputActions.Dash))
				return current;

			var horizontal = Vector2.Zero;
			var correr = false;
			if (_input.IsActionPressed(InputActions.MoveLeft))
			{
				horizontal = new Vector2((-1 * HorizontalSpeed), GlobalVelocity.y);
				correr = true;
			}
			if (_input.IsActionPressed(InputActions.MoveRight))
			{
				horizontal = new Vector2((HorizontalSpeed), GlobalVelocity.y);
				correr = true;
			}
			if (correr)
			{
				GetNode<Kemono>("Slot/Kemono").Correr();
			}
			else
			{
				GetNode<Kemono>("Slot/Kemono").Quieto();
			}
			GlobalVelocity = horizontal;
			return current;
		});
		_fsm.Add(PlayerState.OnAir, (current, player) =>
		{
			if (_wallTime > 0) return current;
			var horizontal = Vector2.Zero;
			if (_input.IsActionPressed(InputActions.MoveLeft) && !_input.IsActionPressed(InputActions.MoveRight))
			{
				horizontal = new Vector2((-1 * HorizontalSpeed), GlobalVelocity.y);
				GlobalVelocity = horizontal;
				return current;
			}
			if (_input.IsActionPressed(InputActions.MoveRight) && !_input.IsActionPressed(InputActions.MoveLeft))
			{
				horizontal = new Vector2((HorizontalSpeed), GlobalVelocity.y);
				GlobalVelocity = horizontal;
				return current;
			}
			return current;
		});
		_fsm.Add(PlayerState.OnAir, (current, player) =>
		{
			if (IsOnFloor())
			{
				GlobalVelocity.x = 0;
				return PlayerState.OnGround;
			}
			return current;
		});
		_fsm.Add(PlayerState.OnAir, (current, player) =>
		{
			if (IsOnWall())
			{
				GlobalVelocity = GravityVector;
				var isRight = _getCollisionNormal() == Vector2.Left;
				return isRight ? PlayerState.OnRightWall : PlayerState.OnLeftWall;
			}
			return current;
		});
		_fsm.Add(PlayerState.OnLeftWall, (current, player) =>
		{
			if (_input.IsActionJustPressed(InputActions.Jump))
			{
				var jump = new Vector2(WallJumpHorizontalForce, -JumpForce);
				GlobalVelocity = jump;
				_wallTime = WallTime;
				return PlayerState.OnAir;
			}
			if (!_input.IsActionPressed(InputActions.MoveLeft) && !_input.IsActionPressed(InputActions.Jump) || !GetNode<Left>("Left").LeftCast())
			{
				return PlayerState.OnAir;
			}
			return current;
		});
		_fsm.Add(PlayerState.OnLeftWall, (current, player) =>
		{
			if (_input.IsActionJustPressed(InputActions.MoveRight))
			{
				var jump = new Vector2(JumpFromWallHorizontalForce, -JumpForce);
				GlobalVelocity = jump;
				_wallTime = 0;
				return PlayerState.OnAir;
			}
			if (!_input.IsActionPressed(InputActions.MoveLeft) && !_input.IsActionPressed(InputActions.Jump) || !GetNode<Left>("Left").LeftCast())
			{
				return PlayerState.OnAir;
			}
			return current;
		});
		_fsm.Add(PlayerState.OnRightWall, (current, player) =>
		{
			if (_input.IsActionJustPressed(InputActions.MoveLeft))
			{
				var jump = new Vector2(JumpFromWallHorizontalForce * -1f, -JumpForce);
				GlobalVelocity = jump;
				_wallTime = 0;
				return PlayerState.OnAir;
			}
			if (!_input.IsActionPressed(InputActions.MoveRight) && !_input.IsActionPressed(InputActions.Jump) || !GetNode<Right>("Right").RightCast())
			{
				return PlayerState.OnAir;
			}
			return current;
		});
		_fsm.Add(PlayerState.OnRightWall, (current, player) =>
		{
			if (_input.IsActionJustPressed(InputActions.Jump))
			{
				var jump = new Vector2(WallJumpHorizontalForce * -1, -JumpForce);
				GlobalVelocity = jump;
				_wallTime = WallTime;
				return PlayerState.OnAir;
			}
			if (!_input.IsActionPressed(InputActions.MoveRight) && !_input.IsActionPressed(InputActions.Jump) || !GetNode<Right>("Right").RightCast())
			{
				return PlayerState.OnAir;
			}
			return current;
		});
		_fsm.Add(PlayerState.OnGround, (current, player) =>
		{
			if (_input.IsActionPressed(InputActions.Dash) && !_dashed || _dashTime > 0)
			{
				_dashed = true;
				var dash = new Vector2(GlobalVelocity.x * DashFactor, GlobalVelocity.y);
				if (_dashTime == 0) _dashTime = 0.25f;
				GlobalVelocity = dash;
			}
			if (!_input.IsActionPressed(InputActions.Dash))
			{
				_dashed = false;
			}
			return current;
		});
		_fsm.Add(PlayerState.OnGround, (current, player) => !IsOnFloor() && !IsOnWall() ? PlayerState.OnAir : current);
	}

	private Vector2 _getCollisionNormal()
	{
		var lastIndex = GetSlideCount() - 1;
		if (lastIndex < 0) return Vector2.Zero;
		var wallNormal = GetSlideCollision(lastIndex).Normal;
		return wallNormal;
	}

	public override void _PhysicsProcess(float delta)
	{
		GlobalVelocity = MoveAndSlide(GlobalVelocity, FloorNormal);
	}
	public override void _Process(float delta)
	{
		_fsm.Tick(this);
		_wallTime = Mathf.Clamp(_wallTime - delta, 0, 10);
		_dashTime = Mathf.Clamp(_dashTime - delta, 0, 10);

		if ((_beforePosition - Position).Normalized().x < 0 && _ScaleHorizontal)
		{
			var transform = GetNode<Node2D>("Slot").Transform;
			transform.x *= -1;
			GetNode<Node2D>("Slot").Transform = transform;
			_ScaleHorizontal = false;
		}

		if ((_beforePosition - Position).Normalized().x > 0 && !_ScaleHorizontal)
		{
			var transform = GetNode<Node2D>("Slot").Transform;
			transform.x *= -1;
			GetNode<Node2D>("Slot").Transform = transform;
			_ScaleHorizontal = true;
		}

		_beforePosition = Position;

		if (Input.IsActionJustPressed("ataque"))
		{
			//GetNode<Kemono>("Slot/Kemono").Rotate(100 * delta);
			//GetNode<ArmaCircular>("Slot/Arma").Atacar();
			//GlobalPosition = GetNode<Position2D>("Slot/Portal").GlobalPosition;

			//GetNode<Tween>("Tween").InterpolateProperty(GetNode<Sprite>("Circulo"), "scale", GetNode<Sprite>("Circulo").Scale, GetNode<Sprite>("Circulo").Scale * 5000, 5f);//no funca
		}
		//else GetNode<Kemono>("Slot/Kemono").Rotation = 0;
	}
}
