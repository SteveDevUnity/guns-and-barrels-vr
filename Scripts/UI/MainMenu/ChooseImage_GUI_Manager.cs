using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseImage_GUI_Manager : MonoBehaviour
{
    [SerializeField] Button[] _profileImageButtons;
    [SerializeField] Button _cancelButton;
    [SerializeField] MeshRenderer _boardMeshrenderer;
    [SerializeField] GameObject _board;

    public static event Action<Sprite> OnImageChoosen;
    public static event Action OnReturnToMainMenu;

    private Canvas _canvas;

    private Sprite _chosenImageSprite;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        MainMenu_GUI_Manager.OnNewGameBoardClicked += OpenGUI;
        ChooseName_GUI_Manager.OnReturnToChooseImage += OpenGUI;
        Warning_GUI_Manager.OnStartNewProfile += OpenGUI;
        ConfirmProfile_GUI_Manager.OnChooseImageButtonPressed += OpenGUI;
    }

    private void OnDisable()
    {
        MainMenu_GUI_Manager.OnNewGameBoardClicked -= OpenGUI;
        ChooseName_GUI_Manager.OnReturnToChooseImage -= OpenGUI;
        Warning_GUI_Manager.OnStartNewProfile -= OpenGUI;
        ConfirmProfile_GUI_Manager.OnChooseImageButtonPressed -= OpenGUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        _canvas.enabled = false;
        _boardMeshrenderer.enabled = false; 

        foreach (Button button in _profileImageButtons)
        {
            button.onClick.AddListener(() => OnProfilImageButtonClicked(button));
        }

        _cancelButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void OpenGUI()
    {
        _canvas.enabled = true;
        _boardMeshrenderer.enabled = true;
        Vector3 directionToCamera = Camera.main.transform.position - _board.transform.position;

        directionToCamera.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

        _board.transform.rotation = targetRotation;

        _board.transform.Rotate(-90, 0, 0);
    }


    // Player clicked on one of the ProfilImage-Buttons
    private void OnProfilImageButtonClicked(Button clickedButton)
    {       
        _chosenImageSprite = clickedButton.gameObject.GetComponent<Image>().sprite;
        _canvas.enabled = false;
        _boardMeshrenderer.enabled = false;

        // Notify: ConfirmProfile_GUI_Manager to update the profile image shown to the user
        OnImageChoosen.Invoke(_chosenImageSprite);              
    }

    private void ReturnToMainMenu()
    {
        _canvas.enabled = false;
        _boardMeshrenderer.enabled = false;

        // Notify: MainMenu_GUI_Manager.cs to show mainmenu sign 
        OnReturnToMainMenu.Invoke();

    }
}
