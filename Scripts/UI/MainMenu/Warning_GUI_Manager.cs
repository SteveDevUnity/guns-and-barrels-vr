using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning_GUI_Manager : MonoBehaviour
{
    [SerializeField] Button _continueButton;
    [SerializeField] Button _cancelButton;
    [SerializeField] MeshRenderer _boardMeshRenderer;
    [SerializeField] GameObject _board;

    private Canvas _canvas;

    public static event Action OnReturnToMainMenu;
    public static event Action OnStartNewProfile;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }


    private void Start()
    {
        _canvas.enabled = false;
        _boardMeshRenderer.enabled = false;
        _continueButton.onClick.AddListener(StartNewProfile);
        _cancelButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void OnEnable()
    {
        MainMenu_GUI_Manager.OnShowWarningMessage += ShowCanvas;
    }

    private void OnDisable()
    {
        MainMenu_GUI_Manager.OnShowWarningMessage -= ShowCanvas;
    }

    private void ShowCanvas()
    {
        _canvas.enabled = true;
        _boardMeshRenderer.enabled = true;
               
        Vector3 directionToCamera = Camera.main.transform.position - _board.transform.position;
       
        directionToCamera.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

        _board.transform.rotation = targetRotation;
         
        _board.transform.Rotate(-90, 0, 0);

    }

    private void ReturnToMainMenu()
    {
        _canvas.enabled = false;
        _boardMeshRenderer.enabled = false;

        // Notify: MainMenu_GUI_Manager.cs to show main menu sign
        OnReturnToMainMenu.Invoke();
    }

    private void StartNewProfile()
    {
        _canvas.enabled = false;
        _boardMeshRenderer.enabled = false;

        // Notify: ChooseImage_GUI_Manager.cs to open choose image GUI. 
        OnStartNewProfile.Invoke();
    }
}
