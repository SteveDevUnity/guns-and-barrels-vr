using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Profile_GUI_Manager : MonoBehaviour, IDataPersistance
{

    [SerializeField] Image _profileImage;
    [SerializeField] TextMeshProUGUI _playerNameText;
    [SerializeField] Button _returnButton;
    [SerializeField] MeshRenderer _boardMeshRenderer;
    [SerializeField] GameObject _board;

    private Canvas _canvas;
    private Sprite _profileImageSprite;
    private string _playerName;

    public static event Action OnReturnToMainMenu;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();    
    }

    private void OnEnable()
    {
        MainMenu_GUI_Manager.OnProfileBoardClicked += OpenProfileGUI;
    }

    private void OnDisable()
    {
        MainMenu_GUI_Manager.OnProfileBoardClicked -= OpenProfileGUI;
    }

    void IDataPersistance.LoadGameData(GameData gameData)
    {
        _profileImageSprite = Resources.Load<Sprite>(gameData.PlayerProfileSprite);
        _playerName = gameData.PlayerName;
    }

    private void Start()
    {
        _canvas.enabled = false;
        _boardMeshRenderer.enabled = false;
        _returnButton.onClick.AddListener(ReturnToMainMenu);
        
    }

    private void OpenProfileGUI()
    {
        _playerNameText.text = _playerName;       
        _profileImage.sprite = _profileImageSprite;
       
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
        OnReturnToMainMenu.Invoke();
    }

    
    
}
