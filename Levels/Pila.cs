using Godot;

public class Pila : StaticBody2D
{
	public void SetearVisible(bool visible)
	{
		GetNode<RichTextLabel>("Mensaje").Visible = visible;
	}
}
