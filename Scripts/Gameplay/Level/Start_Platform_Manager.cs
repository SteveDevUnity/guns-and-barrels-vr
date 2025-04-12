using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Start_Platform_Manager : MonoBehaviour
{
   
    [SerializeField] Animator _animator;

    [SerializeField] MeshRenderer _signMeshRenderer;

    

    public static event Action<bool> OnUserOnStartplatformChanged;

    // Start is called before the first frame update
    void Start()
    {
        _animator.SetBool("SignIsRotating", true);       
    }

    // Player entered Start_Platform
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("User"))
        {
            // Notify: Level_Manager.cs to pause/resume level or show/hide select level GUI
            OnUserOnStartplatformChanged.Invoke(true);

            _animator.SetBool("SignIsRotating", false);
            _signMeshRenderer.enabled = false;
        }

    }


    // User left Start_Platform
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("User"))
        {
            _signMeshRenderer.enabled = true;
            _animator.SetBool("SignIsRotating", true); 

            OnUserOnStartplatformChanged.Invoke(false);
        }

    }
}
