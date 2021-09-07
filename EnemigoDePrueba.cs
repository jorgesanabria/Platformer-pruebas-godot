using Godot;
using System;

public class EnemigoDePrueba : Actor<EnemigoDePruebaEstados>
{
    protected override bool CompareState(EnemigoDePruebaEstados toCompare)
    {
        return toCompare == CurrentState;
    }
}
public enum EnemigoDePruebaEstados
{
	Quieto,
	Caminando,
	Atacando
}
