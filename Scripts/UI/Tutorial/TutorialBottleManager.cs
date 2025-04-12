using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBottleManager : MonoBehaviour
{
    public static event Action OnBottleShot;
    private AudioSource _audioSource;
    private MeshRenderer _meshRenderer;

    //Changed by TutorialManager.cs
    public static bool BottleIsDestructable = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }


    // If player hits the bottle in the tutorial, the door script will be informed to unlock the door
    // and TutorialManager will save tutorialIsCompleted
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.CompareTag("Bullet") && BottleIsDestructable)
        {
            // Notify: TutorialManager.cs to enable open menu_button_tutorial
            OnBottleShot.Invoke();

            StartCoroutine(DelayDestroy());
        }
    }

    private IEnumerator DelayDestroy()
    {
        _meshRenderer.enabled = false;
        yield return new WaitForSeconds(0.1f);
        _audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
