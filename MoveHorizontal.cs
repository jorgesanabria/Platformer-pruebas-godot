using Godot;

public class MoveHorizontal : Node, IMovementStateHandler
{
	[Export]
	public NodePath pathToAnimation;

	[Export]
	public NodePath SaltarDesdePared { get; set; }
	public string HandlerName => nameof(MoveHorizontal);
	bool isRotated;
	public string Handle(CharacterController controller)
	{
		if (Input.IsActionPressed("ui_right"))
		{
			controller.MoveHorizontal(HorizontalDirection.Right);
			GetNode<Kemono>(pathToAnimation).Animation = "correr";
			if (!isRotated)
			{
				GetNode<Kemono>(pathToAnimation).Rotate(0.2f);
				isRotated = true;
			}
		}
		else if (Input.IsActionPressed("ui_left"))
		{
			controller.MoveHorizontal(HorizontalDirection.Left);
			GetNode<Kemono>(pathToAnimation).Animation = "correr";
			if (!isRotated)
			{
				GetNode<Kemono>(pathToAnimation).Rotate(0.2f);
				isRotated = true;
			}
		}
		else
		{
			controller.MoveHorizontal(HorizontalDirection.None);
			GetNode<Kemono>(pathToAnimation).Animation = "Quiero";
			if (isRotated)
			{
				GetNode<Kemono>(pathToAnimation).Rotate(-0.2f);
				isRotated = false;
			}
		}

		if (Input.IsActionPressed("ui_up"))
		{
			return nameof(Junp);
		}

		if (GetNode<RayCast2D>(SaltarDesdePared).IsColliding() && !controller.IsOnFloor())
		{
			GD.Print("saltar desde pared");
			return nameof(SaltoDePared);
		}

		return HandlerName;
	}
}
