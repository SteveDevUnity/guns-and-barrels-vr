using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class CharacterController_Manager : MonoBehaviour
{

    private CharacterController _characterController;
    private XROrigin _xROrigin;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _xROrigin = GetComponent<XROrigin>();
    }

    private void FixedUpdate()
    {
        _characterController.height = _xROrigin.CameraInOriginSpaceHeight + 0.15f;

        Vector3 centerPosition = transform.InverseTransformPoint(_xROrigin.Camera.transform.position);

        // sets the position of the characterController with its collider to the camera

        _characterController.center = new Vector3(centerPosition.x,
            _characterController.height / 2 + _characterController.skinWidth, 
            centerPosition.z);

        // ensures that a physics update is performed in every frame
        // and that the player cannot pass through any obstacles

        _characterController.Move(new Vector3(0.001f, -0.001f, 0.001f));
        _characterController.Move(new Vector3(-0.001f, 0.001f, -0.001f));

        
    }

}
