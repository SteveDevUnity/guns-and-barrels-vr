using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

public class PauseMenu_Manager : MonoBehaviour, IDataPersistance
{
    
    [SerializeField] XROrigin _xROrigin;
    [SerializeField] InputActionReference pressMenueButton_Action;
    [SerializeField] Button _restartButton;
    [SerializeField] Button _resumeButton;
    [SerializeField] Button _exitButton;
    [SerializeField] Button _mainMenuButton;
    [SerializeField] Canvas _canvas;
    [SerializeField] TextMeshProUGUI _restartButtonText;
    
    private bool _canvasIsEnabled;
    public bool isPause;
    public static bool LevelIsPlaying;

    public static event Action OnPauseMenuOpened;

    public static event Action OnPauseMenuResumed;   

    public static event Action OnLevelRestartClicked;

    public static event Action OnBackToMainMenuCkicked;

    public static event Action OnExitButtonClicked;

    public static event Action OnMenuButtonClicked;

    private MeshRenderer _meshRenderer;


    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        pressMenueButton_Action.action.started += ToggleOpenPauseMenu;
        ExitGame_GUI_Manager.OnReturnToPauseMenuClicked += OpenPauseMenu;
    }

    private void OnDisable()
    {
        pressMenueButton_Action.action.started -= ToggleOpenPauseMenu;
        ExitGame_GUI_Manager.OnReturnToPauseMenuClicked -= OpenPauseMenu;
    }

    private void Start()
    {
        _canvas.enabled = false;
        _canvasIsEnabled = false;
        _meshRenderer.enabled = false;
        LevelIsPlaying = false;
        
        _restartButton.onClick.AddListener(OnRestartButtonClick);
        _resumeButton.onClick.AddListener(ResumeToLevel);
        _exitButton.onClick.AddListener(ExitGame);
        _mainMenuButton.onClick.AddListener(BackToMainMenu);
    }

    private void OnDestroy()
    {
        _restartButton.onClick.RemoveListener(OnRestartButtonClick);
        _resumeButton.onClick.RemoveListener(ResumeToLevel);
        _exitButton.onClick.RemoveListener(ExitGame);
        _mainMenuButton.onClick.RemoveListener(BackToMainMenu);
    }
    private void OnRestartButtonClick()
    {
        _canvas.enabled = false;
        _canvasIsEnabled = false;
        isPause = false;
        LevelIsPlaying = false;
        

        //Notify: Level_Manager.cs to initiate level restart
        OnLevelRestartClicked.Invoke();

    
    }

    private void ExitGame()
    {
        
        _canvas.enabled = false;
        _canvasIsEnabled = false;
        _meshRenderer.enabled = false;

        //Notify: ExitGame_GUI_Manager to open Exit Game GUI
        OnExitButtonClicked.Invoke();

        
    }

    private void BackToMainMenu()
    {
        _canvas.enabled = false;
        _meshRenderer.enabled = false;

        // Notify: Scene_Manager.cs to load Main Menu Scene. 
        OnBackToMainMenuCkicked.Invoke();

        
    }
    

    // Gets called if Resume Button is pressed
    public void ResumeToLevel()
    {
        _meshRenderer.enabled = false;
        _canvas.enabled = false;
        _canvasIsEnabled = false;
        
        isPause = false;
        OnPauseMenuResumed.Invoke();

       
    }


    public void ToggleOpenPauseMenu(InputAction.CallbackContext context)
    {
        OpenPauseMenu();

        // Notify other opened Menus to close if player opens the Pause Menu with button press
        // while navigating in other menu. 

        OnMenuButtonClicked.Invoke();
    }

    // Gets opened if User presses Menu-Button
    public void OpenPauseMenu()
    {
        
        if (!_canvasIsEnabled)
        {

            if (!LevelIsPlaying)
            {
                _restartButton.interactable = false;
                _restartButton.GetComponent<Image>().raycastTarget = false;
                
                _restartButtonText.color = Color.gray;

            }

            else
            {
                _restartButton.interactable = true;
                _restartButton.GetComponent<Image>().raycastTarget = true;
                
                _restartButtonText.color = Color.white;
            }

            isPause = true;

            // Notify:
            // Level_Run_Timer_GUI_Manager.cs to pause score and time updates
            // TargetBehaviour.cs to stop moving targets
            OnPauseMenuOpened.Invoke();

            _canvas.enabled = true;
            _meshRenderer.enabled = true;

            CalculateBoardPosition();

            _canvasIsEnabled = true;

        }

        else
        {
            _canvas.enabled = false;
            _canvasIsEnabled = false;
            _meshRenderer.enabled = false;
            isPause = false;

            // Notify:
            // Level_Run_Timer_GUI_Manager.cs to continue score and time updates
            // TargetBehaviour.cs to continue moving targets
            OnPauseMenuResumed.Invoke();
            
        }


    }
  

    private void CalculateBoardPosition()
    {        
        Vector3 xrOriginPosition = _xROrigin.Camera.transform.position;
        Vector3 projectionOnPlane = Vector3.ProjectOnPlane(_xROrigin.Camera.transform.forward, Vector3.down);

        Vector3 offset = projectionOnPlane.normalized * 2.5f + new Vector3(0, -0.2f, 0);

        transform.position = xrOriginPosition + offset;      
        
        transform.LookAt(xrOriginPosition);
        transform.Rotate(-90, 0, 0);

        
    }

    
   




}
