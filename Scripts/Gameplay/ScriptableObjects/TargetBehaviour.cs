using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetBehaviour : MonoBehaviour
{

    public float CurrentSpeed;
    public float CurrentHealth;
    
    private int _damage;
    private int _scorePoints;
    private IMovementStrategy _movementStrategy;
    private MeshRenderer _meshRenderer;
    private BoxCollider _boxCollider;
    private AudioSource _audioSource;
    private AudioClip _audioClip;
    private bool _gameIsPaused;

    public static event Action OnNumberOfTargetsChanged;

    private void OnEnable()
    {
        
        PauseMenu_Manager.OnPauseMenuOpened += PauseGame;
        PauseMenu_Manager.OnPauseMenuResumed += ResumeGame;
        Level_Manager.OnLevelPaused += PauseGame;
        Level_Manager.OnLevelResumed += ResumeGame;
    }

    private void OnDisable()
    {
        
        PauseMenu_Manager.OnPauseMenuOpened -= PauseGame;
        PauseMenu_Manager.OnPauseMenuResumed -= ResumeGame;
        Level_Manager.OnLevelPaused -= PauseGame;
        Level_Manager.OnLevelResumed -= ResumeGame;

    }

    private void PauseGame()
    {
        _gameIsPaused = true;
    }

    private void ResumeGame()
    {
        _gameIsPaused = false;
    }

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _boxCollider = GetComponent<BoxCollider>();
        _audioSource = GetComponent<AudioSource>();
      
    }

    public void InitializeTarget(TargetData targetData, float speedMultiplier, float healthMultiplier, AudioClip explosionSound)
    {
        CurrentSpeed = targetData.BaseMovementSpeed * speedMultiplier;
        CurrentHealth = targetData.BaseHealth * healthMultiplier;
        _damage = targetData.AmountOfDamage;
        _scorePoints = targetData.ScorePoints;
        _audioClip = explosionSound;
    }

    private void Update()
    {
        if (_movementStrategy != null && !_gameIsPaused)
        {
            _movementStrategy.Movement(this);
        }

        else
        {
            return;
        }

    }
   
    public void SetMovementStrategy(IMovementStrategy strategy)
    {
        _movementStrategy = strategy;
    }


    // Gets called if Ray of Colt_Manager.cs hits collider with tag "Target"
    public void HitDetection()
    {
        CurrentHealth -= _damage;
        

        if (CurrentHealth <= 0)
        {
            Level_Manager.CurrentLevelScore += _scorePoints;
            Level_Run_Timer_GUI_Manager.CurrentLevelScore += _scorePoints;
            DestroyTarget();
        }

        else
        {
            return;
        }
    }

    public void DestroyTarget()
    {
        _meshRenderer.enabled = false;
        _boxCollider.enabled = false;

        OnNumberOfTargetsChanged.Invoke();

        StartCoroutine(PlayAudioWhenDestroyed());
       
    }

    private IEnumerator PlayAudioWhenDestroyed()
    {
        yield return new WaitForSeconds(0.1f);
        _audioSource.PlayOneShot(_audioClip);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }


}
