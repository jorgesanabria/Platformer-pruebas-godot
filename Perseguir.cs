using Godot;

public class Perseguir : Node, IStateHandler
{

	[Export]
	public NodePath PathToDetector { get; set; }
	private DetectorRadial _detector;

	public string StateToHandle => nameof(Perseguir);

	public string HandleState(Actor actor)
	{
		var (detectado, point) = _detector.DetectPlayer();

		if (!detectado) return nameof(Mover);

		actor.MoveTo(point);

		return StateToHandle;
	}

	public override void _Ready()
	{
		_detector = GetNode<DetectorRadial>(PathToDetector);
	}
}
