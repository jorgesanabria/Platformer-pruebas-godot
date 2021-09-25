using Godot;

public class Junp : Node, IMovementStateHandler
{
	public string HandlerName => nameof(Junp);

	[Export]
	public float JunpFoce { get; set; } = 700;

	[Export]
	public NodePath Animation { get; set; }

	[Export]
	public NodePath SaltarDesdePared { get; set; }

	private string _beforeAnimation;

	private bool _jumpPermited = true;

	public string Handle(CharacterController controller)
	{
		if (_jumpPermited)
		{
			GetNode<Kemono>(Animation).Animation = "saltando";
			controller.Velocity += new Vector2(controller.Velocity.x, -JunpFoce);
			_jumpPermited = false;
		}

		if (controller.IsOnFloor())
		{
			_jumpPermited = true;
			return nameof(MoveHorizontal);
		}

		//if (controller.IsOnFloor())
		//{
		//	GetNode<Kemono>(Animation).Animation = _beforeAnimation;
		//	controller.MoveHorizontal(HorizontalDirection.None);
		//	return nameof(MoveHorizontal);
		//}
		if (GetNode<RayCast2D>(SaltarDesdePared).IsColliding())
		{
			GD.Print("on saltar desde pared");
			return nameof(SaltoDePared);
		}

		return HandlerName;
	}
	public override void _Ready()
	{
		_beforeAnimation = GetNode<Kemono>(Animation).Animation;
	}
}
