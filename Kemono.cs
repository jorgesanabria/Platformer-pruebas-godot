using Godot;

public class Kemono : AnimatedSprite
{
	public void Correr() => Animation = "Correr";
	public void Quieto() => Animation = "Quieto";
	public void Saltar() => Animation = "Saltar";
}
