using Godot;
using System;

public class OneJointIK : Node
{
	[Export]
	public NodePath UpperBoneNode { get; set; }
	[Export]
	public NodePath LowerBoneNode { get; set; }
	[Export]
	public NodePath TerminusNode { get; set; }
	[Export]
	public bool ReverseJoint { get; set; }
	[Export]
	public float JointExtendLimit { get; set; } = 20f;
	[Export]
	public float JointFoldLimit { get; set; } = 160f;
	[Export]
	public bool PinTerminusRotation { get; set; }
	[Export]
	public float MaxDistance { get; set; } = 280;
	private Node2D _upperBone;
	private Node2D _lowerBone;
	private Node2D _terminus;

	private float _jointSign = 1.0f;
	private float _extendLimit;
	private float _foldLimit;
	private string _configWarning = string.Empty;
	private bool _readied;

	public override void _Ready()
	{
		_upperBone = GetNode<Node2D>(UpperBoneNode);
		_lowerBone = GetNode<Node2D>(LowerBoneNode);
		_terminus = GetNode<Node2D>(TerminusNode);

		if (_lowerBone.GetParent() != _upperBone)
		{
			GD.Print("BONE ERROR");
		}
		else
		{
			_setExtendLimit(JointExtendLimit);
			_setFoldLimit(JointFoldLimit);
			_setJointSign(ReverseJoint);
			_readied = true;
		}
	}

	private void _setExtendLimit(float degrees)
	{
		JointExtendLimit = Mathf.Clamp(Mathf.Abs(degrees), 1.0f, 179.0f);
		_extendLimit = Mathf.Deg2Rad(JointExtendLimit);
		if (_extendLimit >= _foldLimit)
		{
			_configWarning = "Extend Limit must be less than Fold Limit!";
		}
	}
	private void _setFoldLimit(float degrees)
	{
		JointFoldLimit = Mathf.Clamp(Mathf.Abs(degrees), 1.0f, 179.0f);
		_foldLimit = Mathf.Deg2Rad(JointFoldLimit);
		if (_foldLimit <= _extendLimit)
		{
			_configWarning = "Fold Limit must be greater than Extend Limit!";
		}
	}
	private void _setJointSign(bool reverse)
	{
		ReverseJoint = reverse;
		if (ReverseJoint)
		{
			_jointSign = -1.0f;
		}
		else
		{
			_jointSign = 1.0f;
		}
	}

	public override string _GetConfigurationWarning()
	{
		return _configWarning;
	}

	public void ReachToward(Vector2 coordinate)
	{
		if (!_readied) return;

		var terminusRotation = _terminus.GlobalRotation;
		var distance = _upperBone.GlobalPosition.DistanceTo(coordinate);
		var upperLenght = _lowerBone.Position.Length();
		var lowerLenght = _terminus.Position.Length();

		distance = Mathf.Clamp(distance, Mathf.Abs(upperLenght - lowerLenght), upperLenght + lowerLenght);
		distance = Mathf.Clamp(distance, 0, MaxDistance);
		_lowerBone.Rotation = Mathf.Pi - Mathf.Acos((Mathf.Pow(upperLenght, 2) + Mathf.Pow(lowerLenght, 2) - Mathf.Pow(distance, 2)) / (2 * upperLenght * lowerLenght));
		
		if (float.IsNaN(_lowerBone.Rotation)) return;
		_lowerBone.Rotation = Mathf.Clamp(_lowerBone.Rotation, _extendLimit, _foldLimit);
		_lowerBone.Rotation = Mathf.Abs(_lowerBone.Rotation) * _jointSign;

		var alphaVector = _terminus.GlobalPosition - _upperBone.GlobalPosition;
		var goalVector = coordinate - _upperBone.GlobalPosition;
		var rotateBy = alphaVector.AngleTo(goalVector);
		_upperBone.GlobalRotation += rotateBy;

		if (PinTerminusRotation)
		{
			_terminus.GlobalRotation = terminusRotation;
		}
	}
}
