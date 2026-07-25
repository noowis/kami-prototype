using Godot;
using System;
using kamiprototype.Scripts.GameplayClasses;

public class BasicAttack : Action
{
    private Sequence HitSequence = new Sequence([1, 1.5d, 2], ['a', 'b', 'a']);
    public override int ExecuteAction()
    {
        throw new NotImplementedException();
    }
}
