using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementStrategyScriptableObject : ScriptableObject
{
    public abstract IMovementStrategy CreateMovementStreategy();

}
