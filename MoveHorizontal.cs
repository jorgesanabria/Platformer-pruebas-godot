using Godot;

public class MoveHorizontal : Node, IMovementStateHandler
{
	public string HandlerName => nameof(MoveHorizontal);

	public string Handle(CharacterController controller)
	{
		if (Input.IsActionPressed("ui_right"))
		{
			controller.MoveHorizontal(HorizontalDirection.Right);
		}
		else if (Input.IsActionPressed("ui_left"))
		{
			controller.MoveHorizontal(HorizontalDirection.Left);
		}
		else
		{
			controller.MoveHorizontal(HorizontalDirection.None);
		}
		return HandlerName;
	}
}
