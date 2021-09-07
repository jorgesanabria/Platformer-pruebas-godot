using Godot;
using System;

public class Mover : Node, Actor<EnemigoDePruebaEstados>.IStateHandler<EnemigoDePrueba>
{
    public EnemigoDePruebaEstados StateToHandle => EnemigoDePruebaEstados.Caminando;

    public EnemigoDePruebaEstados HandleState(EnemigoDePrueba actor)
    {

        return EnemigoDePruebaEstados.Caminando;
    }
    public override void _Ready()
	{
		
	}
}
