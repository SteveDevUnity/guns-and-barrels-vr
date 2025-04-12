using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TutorialDrawerManager : MonoBehaviour, IDataPersistance
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _xRGrabInteractable;
    private bool _drawerIsUsed;
    private AudioSource _audioSource;
    private ConfigurableJoint _joint;
    private float _minSpeed = 0.002f;
    private float _maxSpeed = 0.4f;

    private float _smoothFactor = 0.1f;
    private Vector3 _lastPosition;
    private float _movementSpeed;

    private bool _tutorialIsCompleted;
    private SoftJointLimit _jointLimit;

    private void Awake()
    {
        _xRGrabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        _audioSource = GetComponent<AudioSource>();
        _drawerIsUsed = false;
        _joint = GetComponent<ConfigurableJoint>();
        _xRGrabInteractable.selectEntered.AddListener(OnUsingDrawer);
        _xRGrabInteractable.selectExited.AddListener(OnDetachDrawer);       
    }

    private void OnEnable()
    {
        TutorialManager.OnGripTutorialConfirmed += ActivateJointLimit;
    }

    private void OnDisable()
    {
        TutorialManager.OnGripTutorialConfirmed -= ActivateJointLimit;
    }

    private void OnDestroy()
    {
        _xRGrabInteractable.selectEntered.RemoveListener(OnUsingDrawer);
        _xRGrabInteractable.selectExited.RemoveListener(OnDetachDrawer);
    }

    void IDataPersistance.LoadGameData(GameData gameData)
    {
        _tutorialIsCompleted = gameData.TutorialIsCompleted;
    }

    private void Start()
    {

        _lastPosition = transform.position;

        if (!_tutorialIsCompleted)
        {
            _jointLimit = _joint.linearLimit;

            _jointLimit.limit = 0.0f;

            _joint.linearLimit = _jointLimit;

        }
    }

    private void Update()
    {      
        float rawSpeed = (transform.position - _lastPosition).magnitude / Time.deltaTime;

        if (rawSpeed < 0.01f)
        {
            rawSpeed = 0.0f;
        }

        _movementSpeed = Mathf.Lerp(_movementSpeed, rawSpeed, _smoothFactor);        
        
        if (_movementSpeed > _minSpeed && _drawerIsUsed)
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }

            _audioSource.volume = Mathf.Clamp01(_movementSpeed / _maxSpeed);
        }

        else
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();
        }

        _lastPosition = transform.position;
    }




    private void OnUsingDrawer(SelectEnterEventArgs selectEnterEventArgs)
    {
        _drawerIsUsed = true;
    }

    private void OnDetachDrawer(SelectExitEventArgs selectExitEventArgs)
    {
        _drawerIsUsed = false;        
    }

   
    private void ActivateJointLimit()
    {
        _jointLimit.limit = 0.4f;

        _joint.linearLimit = _jointLimit;
    }



}
