using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMovement : IMovementStrategy
{

    private Vector3 _direction;
   
    public LinearMovement(Vector3 initialDirection)
    {
        _direction = initialDirection;
    }

    public void Movement(TargetBehaviour target)
    {

        target.transform.Translate(_direction * target.CurrentSpeed * Time.deltaTime * 0.25f, Space.World);

        
        Ray ray = new Ray(target.transform.position, _direction);
        RaycastHit hit;

        
        if (Physics.Raycast(ray, out hit, 0.2f))
        {

            // Reflection of the direction
            _direction = Vector3.Reflect(_direction, hit.normal);
        }
    }

    
}
