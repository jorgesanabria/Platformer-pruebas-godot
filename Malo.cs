using Godot;
using MaquinaDeEstados;
using MaquinaDeEstados.BT;
using MaquinaDeEstados.FSM;
using MaquinaDeEstados.InputHandler;
using System;

public class Malo : KinematicBody2D, IDamagable
{
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
	private float _wallTime = 0f;
	[Export]
	public float WallTime = 0.5f;
	[Export]
	public NodePath TextPath;
	RichTextLabel _textLabel;
	protected FiniteStateMachine<PlayerState, Malo> _fsm;
	protected AIInputHandler<InputActions> _input;
	protected Root<Malo> _bt;
	protected FiniteStateMachine<EnemyState, Malo> _enemyState;
	private float _velocidadNormal = 0f;
	EnemyState _estadoAnterior;
	private float _caminarHasta = 0;
	private float _xInicial;
	private float _xOriginal;
	[Export]
	public float MaximoPatrullaAlerta = 100;
	private float _tiempoAlerta;
	public override void _Ready()
	{
		_xInicial = Position.x;
		_xOriginal = _xInicial;
		_textLabel = GetNode(TextPath) as RichTextLabel;
		_input = new AIInputHandler<InputActions>();
		_fsm = new FiniteStateMachine<PlayerState, Malo>(equalizer: (current, captured) => current == captured) { InitialState = PlayerState.OnAir };
		_enemyState = new FiniteStateMachine<EnemyState, Malo>(equalizer: (current, captured) => current == captured) { InitialState = EnemyState.MoviendoDerecha };
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
				return PlayerState.OnAir;
			}
			return current;
		});
		_fsm.Add(PlayerState.OnGround, (current, player) =>
		{
			var horizontal = Vector2.Zero;
			if (_input.IsActionPressed(InputActions.MoveLeft))
			{
				horizontal = new Vector2((-1 * HorizontalSpeed), GlobalVelocity.y);
			}
			if (_input.IsActionPressed(InputActions.MoveRight))
			{
				horizontal = new Vector2((HorizontalSpeed), GlobalVelocity.y);
			}
			GlobalVelocity = horizontal;
			return current;
		});
		_fsm.Add(PlayerState.OnAir, (current, player) =>
		{
			if (_wallTime > 0) return current;
			var horizontal = Vector2.Zero;
			if (_input.IsActionPressed(InputActions.MoveLeft))
			{
				horizontal = new Vector2((-1 * HorizontalSpeed), GlobalVelocity.y);
				GlobalVelocity = horizontal;
			}
			if (_input.IsActionPressed(InputActions.MoveRight))
			{
				horizontal = new Vector2((HorizontalSpeed), GlobalVelocity.y);
				GlobalVelocity = horizontal;
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
				var isLef = _getCollisionNormal() == Vector2.Left;
				return isLef ? PlayerState.OnRightWall : PlayerState.OnLeftWall;
			}
			return current;
		});
		_fsm.Add(PlayerState.OnLeftWall, (current, delta) => IsOnFloor() ? PlayerState.OnGround : current);
		_fsm.Add(PlayerState.OnRightWall, (current, delta) => IsOnFloor() ? PlayerState.OnGround : current);
		_fsm.Add(PlayerState.OnLeftWall, (current, delta) =>
		{
			if (_input.IsActionPressed(InputActions.MoveRight))
			{
				var jump = new Vector2(HorizontalSpeed, -JumpForce);
				GlobalVelocity = jump;
				return PlayerState.OnAir;
			}
			return current;
		});
		_fsm.Add(PlayerState.OnRightWall, (current, player) =>
		{
			if (_input.IsActionPressed(InputActions.MoveLeft))
			{
				var jump = new Vector2(HorizontalSpeed * -1, -JumpForce);
				GlobalVelocity = jump;
				return PlayerState.OnAir;
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
			return current;
		});

		_enemyState.Add(EnemyState.MoviendoDerecha, (current, enemy) =>
		{
			_estadoAnterior = current;
			Player jugador = DetectarYPerseguir();

			if (_caminarHasta != 0)
			{
				HorizontalSpeed = _velocidadNormal * 4;
			}
			if (Mathf.Abs(Position.x - _caminarHasta) <= 10)
			{
				HorizontalSpeed = _velocidadNormal;

				if (_caminarHasta != 0)
				{
					_caminarHasta = 0;
					_xInicial = Position.x;
					_tiempoAlerta = 30;
					return EnemyState.AlertaDerecha;
				}
			}

			if (jugador != null && Mathf.Abs(Position.x - jugador.Position.x) <= 50)
			{
				_input.SetActionReleased(InputActions.MoveRight);
				return EnemyState.Atacando;
			}

			if (IsOnWall())
			{
				_input.SetActionPressed(InputActions.MoveLeft);
				_input.SetActionReleased(InputActions.MoveRight);
				return EnemyState.MoviendoIzquierda;
			}
			_input.SetActionPressed(InputActions.MoveRight);
			_input.SetActionReleased(InputActions.MoveLeft);

			var bodies = GetNode<Area2D>("Area2D2")?.GetOverlappingBodies() ?? new Godot.Collections.Array();

			foreach (var body in bodies)
			{
				jugador = body as Player;
				if (jugador != null && !jugador.Cubierto)
				{
					GD.Print($"{DateTime.Now.ToString("G")}");
					//_input.SetActionPressed(InputActions.MoveRight);
					//_input.SetActionReleased(InputActions.MoveLeft);
					return EnemyState.MoviendoIzquierda;
				}
			}
			return current;
		});

		_enemyState.Add(EnemyState.MoviendoIzquierda, (current, enemy) =>
		{
			_estadoAnterior = current;
			Player jugador = DetectarYPerseguir();

			if (_caminarHasta != 0)
			{
				HorizontalSpeed = _velocidadNormal * 4;
			}
			if (Mathf.Abs(Position.x - _caminarHasta) <= 10)
			{
				HorizontalSpeed = _velocidadNormal;

				if (_caminarHasta != 0)
				{
					_caminarHasta = 0;
					_xInicial = Position.x;
					_tiempoAlerta = 30;
					return EnemyState.AlertaIzquierda;
				}
			}

			if (jugador != null && Mathf.Abs(Position.x - jugador.Position.x) <= 50)
			{
				_input.SetActionReleased(InputActions.MoveLeft);
				return EnemyState.Atacando;
			}

			if (IsOnWall())
			{
				_input.SetActionPressed(InputActions.MoveRight);
				_input.SetActionReleased(InputActions.MoveLeft);
				return EnemyState.MoviendoDerecha;
			}
			_input.SetActionPressed(InputActions.MoveLeft);
			_input.SetActionReleased(InputActions.MoveRight);

			var bodies = GetNode<Area2D>("Area2D2")?.GetOverlappingBodies() ?? new Godot.Collections.Array();

			foreach (var body in bodies)
			{
				jugador = body as Player;
				if (jugador != null && !jugador.Cubierto)
				{
					GD.Print($"{DateTime.Now.ToString("G")}");
					//_input.SetActionPressed(InputActions.MoveLeft);
					//_input.SetActionReleased(InputActions.MoveRight);
					return EnemyState.MoviendoDerecha;
				}
			}
			return current;
		});

		_enemyState.Add(EnemyState.AlertaIzquierda, (current, enemy) =>
		{
			_ = DetectarYPerseguir();

			var bodies = GetNode<Area2D>("Area2D2")?.GetOverlappingBodies() ?? new Godot.Collections.Array();

			foreach (var body in bodies)
			{
				if (body is Player jugador && !jugador.Cubierto)
				{
					GD.Print($"{DateTime.Now:G}");
					_input.SetActionPressed(InputActions.MoveRight);
					_input.SetActionReleased(InputActions.MoveLeft);
					return EnemyState.AlertaDerecha;
				}
			}

			if (_caminarHasta != 0)
			{
				_xInicial = _xOriginal;
				return EnemyState.MoviendoIzquierda;
			}
			if (Mathf.Abs(_xInicial - Position.x) <= MaximoPatrullaAlerta)
			{
				_input.SetActionPressed(InputActions.MoveLeft);
				_input.SetActionReleased(InputActions.MoveRight);
			}
			else
			{
				_input.SetActionPressed(InputActions.MoveRight);
				_input.SetActionReleased(InputActions.MoveLeft);
				return EnemyState.AlertaDerecha;
			}
			if (_tiempoAlerta == 0)
			{
				_xInicial = _xOriginal;
				HorizontalSpeed = _velocidadNormal;
				return EnemyState.MoviendoDerecha;
			}

			if (IsOnWall())
			{
				_input.SetActionPressed(InputActions.MoveRight);
				_input.SetActionReleased(InputActions.MoveLeft);
				return EnemyState.AlertaDerecha;
			}

			return current;
		});

		_enemyState.Add(EnemyState.AlertaDerecha, (current, enemy) =>
		{
			_ = DetectarYPerseguir();

			var bodies = GetNode<Area2D>("Area2D2")?.GetOverlappingBodies() ?? new Godot.Collections.Array();

			foreach (var body in bodies)
			{
				if (body is Player jugador && !jugador.Cubierto)
				{
					GD.Print($"{DateTime.Now:G}");
					_input.SetActionPressed(InputActions.MoveLeft);
					_input.SetActionReleased(InputActions.MoveRight);
					return EnemyState.AlertaIzquierda;
				}
			}

			if (_caminarHasta != 0)
			{
				_xInicial = _xOriginal;
				return EnemyState.MoviendoDerecha;
			}
			if (Mathf.Abs(_xInicial - Position.x) <= MaximoPatrullaAlerta)
			{
				_input.SetActionPressed(InputActions.MoveRight);
				_input.SetActionReleased(InputActions.MoveLeft);
			}
			else
			{
				_input.SetActionPressed(InputActions.MoveLeft);
				_input.SetActionReleased(InputActions.MoveRight);
				return EnemyState.AlertaIzquierda;
			}
			if (_tiempoAlerta == 0)
			{
				_xInicial = _xOriginal;
				HorizontalSpeed = _velocidadNormal;
				return EnemyState.MoviendoIzquierda;
			}

			if (IsOnWall())
			{
				_input.SetActionPressed(InputActions.MoveLeft);
				_input.SetActionReleased(InputActions.MoveRight);
				return EnemyState.AlertaIzquierda;
			}

			return current;
		});

		_enemyState.Add(EnemyState.Atacando, (current, enemy) =>
		{
			var bodies = GetNode<Area2D>("Area2D").GetOverlappingBodies() ?? new Godot.Collections.Array();

			Player jugador = null;

			foreach (var body in bodies)
			{
				jugador = body as Player;
				if (body is Player && !jugador.Cubierto)
				{
					if (Mathf.Abs(Position.x - jugador.Position.x) > 100)
					{
						return _estadoAnterior;
					}
				}
			}
			if (jugador == null) return _estadoAnterior;

			return current;
		});

		_velocidadNormal = HorizontalSpeed;
	}

	private Player DetectarYPerseguir()
	{
		var bodies = GetNode<Area2D>("Area2D").GetOverlappingBodies() ?? new Godot.Collections.Array();

		Player jugador = null;

		foreach (var body in bodies)
		{
			jugador = body as Player;
			if (body is Player && !jugador.Cubierto)
			{
				HorizontalSpeed = _velocidadNormal * 4;
				_caminarHasta = jugador.Position.x;
				break;
			}
			else
			{
				HorizontalSpeed = _velocidadNormal;
			}
		}

		return jugador;
	}

	private Vector2 _getCollisionNormal()
	{
		var lastIndex = GetSlideCount() - 1;
		var wallNormal = GetSlideCollision(lastIndex).Normal;
		return wallNormal;
	}

	public override void _PhysicsProcess(float delta)
	{
		_fsm.Tick(this);
		GlobalVelocity = MoveAndSlide(GlobalVelocity, FloorNormal);
		_wallTime = Mathf.Clamp(_wallTime - delta, 0, 10);
		//_bt.Tick(this);
		_enemyState.Tick(this);

		_tiempoAlerta = Mathf.Clamp(_tiempoAlerta - delta, 0, 30);

		if (_tiempoAlerta != 0)
		{
			if (HorizontalSpeed != (_velocidadNormal * 4))
				HorizontalSpeed = _velocidadNormal * 4;
		}

		if (_input.IsActionPressed(InputActions.MoveLeft))
		{
			var angle = GetNode<CollisionShape2D>("Area2D/Detector").Transform.Rotation;
			if (angle < 0) {
				var trasnform = GetNode<CollisionShape2D>("Area2D/Detector").Transform;
				trasnform.Rotation = -angle;
				GetNode<CollisionShape2D>("Area2D/Detector").Transform = trasnform;
			}
			var angle2 = GetNode<CollisionShape2D>("Area2D2/Detector2").Transform.Rotation;
			if (angle2 > 0) {
				var trasnform = GetNode<CollisionShape2D>("Area2D2/Detector2").Transform;
				trasnform.Rotation = -angle2;
				GetNode<CollisionShape2D>("Area2D2/Detector2").Transform = trasnform;
			}
		}
		if (_input.IsActionPressed(InputActions.MoveRight))
		{
			var angle = GetNode<CollisionShape2D>("Area2D/Detector").Transform.Rotation;
			if (angle > 0) {
				var trasnform = GetNode<CollisionShape2D>("Area2D/Detector").Transform;
				trasnform.Rotation = -angle;
				GetNode<CollisionShape2D>("Area2D/Detector").Transform = trasnform;
			}
			var angle2 = GetNode<CollisionShape2D>("Area2D2/Detector2").Transform.Rotation;
			if (angle2 < 0) {
				var trasnform = GetNode<CollisionShape2D>("Area2D2/Detector2").Transform;
				trasnform.Rotation = -angle2;
				GetNode<CollisionShape2D>("Area2D2/Detector2").Transform = trasnform;
			}
		}

	}
	int _life = 100;
	public void Damage()
	{
		GD.Print("estoy lastimado");
		_life -= 10;
		_textLabel.Text = $"Vida {_life}";
	}
}
public interface IDamagable
{
	void Damage();
}
