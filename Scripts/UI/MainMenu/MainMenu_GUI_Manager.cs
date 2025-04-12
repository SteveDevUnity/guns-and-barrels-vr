using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR.Features.Meta;

public class MainMenu_GUI_Manager : MonoBehaviour, IDataPersistance
{   
    [SerializeField] Canvas _mainMenuCanvas;

    [SerializeField] TextMeshProUGUI _continueText;
    [SerializeField] TextMeshProUGUI _profileText;
    [SerializeField] TextMeshProUGUI _newGameText;
    [SerializeField] TextMeshProUGUI _optionsText;

    [SerializeField] XRSimpleInteractable _continueBoardInteractable;
    [SerializeField] XRSimpleInteractable _profileBoardInteractable;
    [SerializeField] XRSimpleInteractable _newGameBoardInteractable;    
    [SerializeField] XRSimpleInteractable _optionsBoardInteractable;

    [SerializeField] Color _standardTextColor;
    [SerializeField] Color _hoveredTextColor;
    [SerializeField] Color _textDisabledColor;  

    private MeshRenderer _parentMeshrenderer;

    private bool _profileIsCreated;

    public static event Action OnNewGameBoardClicked;
    public static event Action OnOptionsBoardClicked;
    public static event Action OnProfileBoardClicked;
    public static event Action OnShowWarningMessage;

    private void Awake()
    {
        _parentMeshrenderer = GetComponent<MeshRenderer>();

    }


    private void OnEnable()
    {
        DataPersistanceManager.OnCheckProfileIsCreated += ProfileStatusChanged;
        OptionsMenu_GUI_Manager.OnOptionsGUIClosed += ShowSign;
        Profile_GUI_Manager.OnReturnToMainMenu += ShowSign;
        ChooseImage_GUI_Manager.OnReturnToMainMenu += ShowSign;
        Warning_GUI_Manager.OnReturnToMainMenu += ShowSign;
        
    }

    private void OnDisable()
    {
        DataPersistanceManager.OnCheckProfileIsCreated -= ProfileStatusChanged;
        OptionsMenu_GUI_Manager.OnOptionsGUIClosed -= ShowSign;
        Profile_GUI_Manager.OnReturnToMainMenu -= ShowSign;
        ChooseImage_GUI_Manager.OnReturnToMainMenu -= ShowSign;
        Warning_GUI_Manager.OnReturnToMainMenu -= ShowSign;
        
    }


    // Start is called before the first frame update
    void Start()
    {
        _newGameBoardInteractable.selectEntered.AddListener(NewGameBoardSelected);
        _newGameBoardInteractable.hoverEntered.AddListener(NewGameBoardHovered);
        _newGameBoardInteractable.hoverExited.AddListener(OnNewGameHoverExited);

        _continueBoardInteractable.selectEntered.AddListener(ContinueBoardClicked);
        _continueBoardInteractable.hoverEntered.AddListener(ContinueBoardHovered);
        _continueBoardInteractable.hoverExited.AddListener(OnContinueHoverExited);

        _profileBoardInteractable.selectEntered.AddListener(ProfileBoardClicked);
        _profileBoardInteractable.hoverEntered.AddListener(ProfileBoardHovered);
        _profileBoardInteractable.hoverExited.AddListener(OnProfileHoverExited);

        _optionsBoardInteractable.selectEntered.AddListener(OptionsBoardClicked);
        _optionsBoardInteractable.hoverEntered.AddListener(OptionsBoardHovered);
        _optionsBoardInteractable.hoverExited.AddListener(OnOptionsHoverExited);

    }

   
    // Checks if user already created a profile and if not continue- and profileboards are disabled
    private void UpdateBoardStates()
    {

        if (!_profileIsCreated)
        {
            _continueBoardInteractable.enabled = false;
            _profileBoardInteractable.enabled = false;

            _continueText.color = _textDisabledColor;
            _profileText.color = _textDisabledColor;
            
        }

        else
        {
            _continueBoardInteractable.enabled = true;
            _profileBoardInteractable.enabled = true;
            _continueText.color = _standardTextColor;
            _profileText.color = _standardTextColor;
        }

    }

    private void NewGameBoardHovered(HoverEnterEventArgs args)
    {
        _newGameText.color = _hoveredTextColor;
    }

    private void OnNewGameHoverExited(HoverExitEventArgs args)
    {
        _newGameText.color = _standardTextColor;
    }

    private void NewGameBoardSelected(SelectEnterEventArgs args)
    {
        HideSign();

        if(_profileIsCreated)
        {
            // Notify: Warning_GUI_Manager.cs to show Warning Message to player
            OnShowWarningMessage.Invoke();
        }

        else
        {
            // Notify:
            // ChooseImage_GUI_Manager.cs to open Choose Image GUI
            OnNewGameBoardClicked.Invoke();
        }
        
    }



    private void ContinueBoardHovered(HoverEnterEventArgs args)
    {        
        _continueText.color = _hoveredTextColor;
    }

    private void OnContinueHoverExited(HoverExitEventArgs args)
    {
        _continueText.color = _standardTextColor;
    }

    private void ContinueBoardClicked(SelectEnterEventArgs args)
    {
        SceneManager.LoadScene("LoadingSCreen_01");
    }


    private void OptionsBoardHovered(HoverEnterEventArgs args)
    {
        _optionsText.color = _hoveredTextColor;
    }

    private void OnOptionsHoverExited(HoverExitEventArgs args)
    {
        _optionsText.color = _standardTextColor;
    }

    private void OptionsBoardClicked(SelectEnterEventArgs args)
    {
        HideSign();
        OnOptionsBoardClicked.Invoke();
    }


    private void ProfileBoardHovered(HoverEnterEventArgs args)
    {
        _profileText.color = _hoveredTextColor;
    }

    private void OnProfileHoverExited(HoverExitEventArgs args)
    {
        _profileText.color = _standardTextColor;
    }

    private void ProfileBoardClicked(SelectEnterEventArgs args)
    {
        HideSign();
        OnProfileBoardClicked.Invoke();
    }

    private void ProfileStatusChanged(bool profileIsCreated)
    {
        _profileIsCreated = profileIsCreated;
        ShowSign();
    }

    private void HideSign()
    {
        _mainMenuCanvas.enabled = false;
        _parentMeshrenderer.enabled = false;

        _continueBoardInteractable.enabled = false;
        _newGameBoardInteractable.enabled = false;
        _profileBoardInteractable.enabled = false;
        _optionsBoardInteractable.enabled = false;
        _mainMenuCanvas.enabled = false;
        _parentMeshrenderer.enabled = false;

    }


    private void ShowSign()
    {
        _mainMenuCanvas.enabled = true;
        
        UpdateBoardStates();

        _parentMeshrenderer.enabled = true;

        _newGameBoardInteractable.enabled = true;
        _optionsBoardInteractable.enabled = true;
        
    }

}
