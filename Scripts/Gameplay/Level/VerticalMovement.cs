using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMovement : IMovementStrategy
{
    public void Movement(TargetBehaviour target)
    {
        Debug.Log("Vertical Movement");
    }
}

