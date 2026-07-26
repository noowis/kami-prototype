using Godot;
using System;
using kamiprototype.Scripts.GameplayClasses;
using kamiprototype.Scripts.Handlers;

public class BasicAttack : Action
{
    private Sequence HitSequence = new Sequence([0.5, 0.75, 0.4], ['b', 'a', 'a']);
    public override int ExecuteAction(HighwayHandler highway)
    {
        highway.LoadSequence(HitSequence);
        highway.Start();
        return 0;
    }
}
