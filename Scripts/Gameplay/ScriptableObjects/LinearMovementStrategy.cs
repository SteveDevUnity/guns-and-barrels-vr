using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MovementStrategy")]
public class LinearMovementStrategy : MovementStrategyScriptableObject
{

    public Vector3 InitialDirection;

    public override IMovementStrategy CreateMovementStreategy()
    {
        return new LinearMovement(InitialDirection);
    }

}
