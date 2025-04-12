using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Manager : MonoBehaviour
{
    private float _maxLifeTime = 1.5f;

    private void Start()
    {
        // If the bullet doesnt hit any target it should be destroyed after maxLifeTime;
        Destroy(gameObject, _maxLifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        if(collision.gameObject.CompareTag("Target"))
        {
            TargetBehaviour target = collision.gameObject.GetComponent<TargetBehaviour>();
            target.HitDetection();
            Destroy(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
        
    }
}
