using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.XR.Interaction.Toolkit.Filtering;

public class Belt_Manager : MonoBehaviour, IDataPersistance
{
    [SerializeField] XROrigin _xr_Origin;
    private CharacterController _characterController;

    [SerializeField] InputActionReference _snapTurn;
    [SerializeField] UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor _socketInteractor;
    private Vector3 _charControllerPosition;
    private Vector3 _fixedPosition;

    private float _angleDifference;
    private float _rotationTreshold = 60.0f;
    private Vector3 _lastForward;
    private bool _tutorialIsCompleted;

   

    private void Awake()
    {
        _characterController = _xr_Origin.GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        Colt_Manager.OnWeaponGrabbedFirstTime += ActivateSocket;
    }

    private void OnDisable()
    {
        Colt_Manager.OnWeaponGrabbedFirstTime -= ActivateSocket;
    }

    void IDataPersistance.LoadGameData(GameData gameData)
    {
        _tutorialIsCompleted = gameData.TutorialIsCompleted;

        if (!_tutorialIsCompleted)
        {
            _socketInteractor.socketActive = false;         
        }

        else
        {
            _socketInteractor.socketActive= true;
        }
               
    }

    private void Start()
    {
        _lastForward = _xr_Origin.Camera.transform.forward;

        _charControllerPosition = _characterController.center;

        _fixedPosition = _characterController.transform.TransformPoint(_charControllerPosition);

        transform.position = new Vector3(_fixedPosition.x, _fixedPosition.y, _fixedPosition.z);


    }

    void Update()
    {

        // The difference in degrees between the initial transform.forward from camera, and current transform.forward camera
        _angleDifference = Vector3.Angle(_lastForward, _xr_Origin.Camera.transform.forward);


        // Rotate the holster with the player only if he turns head more than 60 degrees
        if (_angleDifference > _rotationTreshold)
        {
            transform.eulerAngles = new Vector3(0, _xr_Origin.Camera.transform.eulerAngles.y, 0);
            _lastForward = _xr_Origin.Camera.transform.forward;
        }


        // If player looks down the Holster should be at the LookAt Position of the player
        if (_xr_Origin.Camera.transform.eulerAngles.x < 35.0f || _xr_Origin.Camera.transform.eulerAngles.x > 90.0f)
        {
            transform.eulerAngles = new Vector3(0, _xr_Origin.Camera.transform.eulerAngles.y, 0);
            _lastForward = _xr_Origin.Camera.transform.forward;
        }

        _charControllerPosition = _characterController.center;

        _fixedPosition = _characterController.transform.TransformPoint(_charControllerPosition);

        transform.position = new Vector3(_fixedPosition.x, _fixedPosition.y, _fixedPosition.z);

    }


    private void ActivateSocket()
    {
        _socketInteractor.socketActive = true;
    }





}



