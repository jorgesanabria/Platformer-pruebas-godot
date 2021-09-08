using Godot;
using System.Collections.Generic;
using System.Linq;

public class Mover : Node, IStateHandler
{
	[Export]
	public NodePath WayPointsPath;
	private List<WayPoint> _WayPoints = new List<WayPoint>();
	private WayPoint _CurrentWayPoint;
	public string StateToHandle => nameof(Mover);

	public string HandleState(Actor actor)
	{
		if (_CurrentWayPoint != null && _CurrentWayPoint.DistanceTo(actor) <= 15)
		{
			GD.Print("distancia alcanzada");
			_CurrentWayPoint = _CurrentWayPoint.NextWayPoint(_WayPoints);
		}

		GD.Print(_CurrentWayPoint.Name);

		actor.MoveTo(_CurrentWayPoint);

		return StateToHandle;
	}
	public override void _Ready()
	{
		if (WayPointsPath == null) return;
		foreach (var wayPint in GetNode(WayPointsPath).GetChildren()) _WayPoints.Add((WayPoint)wayPint);

		_CurrentWayPoint = _WayPoints.First();
	}
}
