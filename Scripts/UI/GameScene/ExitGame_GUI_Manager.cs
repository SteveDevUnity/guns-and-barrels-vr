using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ExitGame_GUI_Manager : MonoBehaviour
{
    [SerializeField] Button _confirmButton;
    [SerializeField] Button _cancelButton;
    [SerializeField] XROrigin _xROrigin;
    [SerializeField] Canvas _canvas;  

    private MeshRenderer _meshRenderer;
    
    public static event Action OnReturnToPauseMenuClicked;


    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        PauseMenu_Manager.OnExitButtonClicked += OpenExitGameGUI;
        PauseMenu_Manager.OnMenuButtonClicked += CloseExitGameGUI;
    }

    private void OnDisable()
    {
        PauseMenu_Manager.OnExitButtonClicked -= OpenExitGameGUI;
        PauseMenu_Manager.OnMenuButtonClicked -= CloseExitGameGUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        _canvas.enabled = false;
        _meshRenderer.enabled = false;
        _confirmButton.onClick.AddListener(OnExitGame);
        _cancelButton.onClick.AddListener(OnResumeToPauseMenu);
    }

    
    private void OnExitGame()
    {
        Application.Quit();
    }

    private void OnResumeToPauseMenu()
    {
        _canvas.enabled = false;
        _meshRenderer.enabled = false;
     

        //Notify: PauseMenu_Manager.cs to open PauseMenu GUI again. 
        OnReturnToPauseMenuClicked.Invoke();

    }

    public void OpenExitGameGUI()
    {
        CalculateBoardPosition();
        _meshRenderer.enabled = true;    
        _canvas.enabled = true;
    }

    public void CloseExitGameGUI()
    {
        _canvas.enabled = false;
        _meshRenderer.enabled = false;
    }

    private void CalculateBoardPosition()
    {
        Vector3 xrOriginPosition = _xROrigin.Camera.transform.position;
        Vector3 projectionOnPlane = Vector3.ProjectOnPlane(_xROrigin.Camera.transform.forward, Vector3.down);

        Vector3 offset = projectionOnPlane.normalized * 3.25f;

        transform.position = xrOriginPosition + offset;

        transform.LookAt(xrOriginPosition);
        transform.Rotate(-90, 0, 0);


    }
}
