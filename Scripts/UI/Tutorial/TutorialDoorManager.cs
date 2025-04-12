using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class TutorialDoorManager : MonoBehaviour, IDataPersistance
{
    public AudioClip LockedSound;
    public AudioClip DoorOpenSound;
    public GameObject DoorHandle;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _xRGrabInteractable;
    private bool _tutorialIsCompleted;
    private HingeJoint _joint;
    private Rigidbody _rigidBodyDoor;
    private bool _doorIsLocked;
    private AudioSource _audioSource;
    private Animator _animator;


    private void Awake()
    {
        _xRGrabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        _rigidBodyDoor = GetComponent<Rigidbody>();
        _joint = GetComponent<HingeJoint>();
        _audioSource = GetComponent<AudioSource>();
        _xRGrabInteractable.selectEntered.AddListener(DoorIsUsed);
        _xRGrabInteractable.selectExited.AddListener(DoorDetached);
        _animator = DoorHandle.GetComponent<Animator>();
    }


    private void OnDestroy()
    {
        _xRGrabInteractable.selectEntered.RemoveListener(DoorIsUsed);
        _xRGrabInteractable.selectExited.RemoveListener(DoorDetached);
    }

    void IDataPersistance.LoadGameData(GameData gameData)
    {
        _tutorialIsCompleted = gameData.TutorialIsCompleted;

        if (_tutorialIsCompleted)
        {
            UnlockDoor();
            _doorIsLocked = false;

        }

        else
        {
            LockDoor();
            _doorIsLocked = true;
        }
    }
    private void OnEnable()
    {      
        TutorialManager.OnTutorialCompleted += UnlockDoor;
    }

    private void OnDisable()
    {
        TutorialManager.OnTutorialCompleted -= UnlockDoor;
    }


    private void Start()
    {
        _rigidBodyDoor.mass = 1000f;

    }


    private void UnlockDoor()
    {
        JointLimits limits = _joint.limits;

        limits.min = -105.0f;
        limits.max = 0;

        _joint.limits = limits;

        _doorIsLocked = false;
    }

    private void LockDoor()
    {
        JointLimits limits = _joint.limits;

        limits.min = 0;
        limits.max = 0;

        _joint.limits = limits;
    }

    private void DoorIsUsed(SelectEnterEventArgs args)
    {
        if (_doorIsLocked)
        {
            _audioSource.volume = 0.2f;
            _audioSource.PlayOneShot(LockedSound);
        }

        else
        {
            _animator.SetBool("HandleIsUsed", true);
            _audioSource.volume = 0.2f;
            _audioSource.pitch = 1.3f;
            _audioSource.PlayOneShot(DoorOpenSound);
        }
    }

    private void DoorDetached(SelectExitEventArgs args)
    {
        _animator.SetBool("HandleIsUsed", false);
        _audioSource.volume = 0.1f;
        _audioSource.pitch = 1.2f;
        _audioSource.PlayOneShot(DoorOpenSound);
    }

}
