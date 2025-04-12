using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectSelectManager : UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor
{
    [SerializeField] InputActionProperty defaultSelection;
    [SerializeField] InputActionProperty farSelection;
    
    protected override void Start()
    {
        farSelection.action.Enable();
        defaultSelection.action.Enable();
    }
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (args.interactableObject.transform.gameObject.CompareTag("MainMenuBoard"))
        {
            defaultSelection.action.Disable();
            farSelection.action.Enable();
        }

    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        defaultSelection.action.Enable();
        farSelection.action.Disable();
    }

}
