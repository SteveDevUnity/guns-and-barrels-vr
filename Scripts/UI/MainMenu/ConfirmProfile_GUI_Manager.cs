using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmProfile_GUI_Manager : MonoBehaviour, IDataPersistance
{
    [SerializeField] TextMeshProUGUI _playerName;
    [SerializeField] Button _confirmButton;
    [SerializeField] Button _returnButton;
    [SerializeField] Button _profilImageButton;
    [SerializeField] MeshRenderer _boardMeshRenderer;
    [SerializeField] GameObject _board;

    private Sprite _initialSpriteProfilImageButton;
    private Canvas _canvas;
    private string _enteredName;
    private Sprite _enteredImage;

    public static event Action OnReturnToChooseName;
    public static event Action OnChooseImageButtonPressed;

    private Image _profileImage;

    private bool _overrideNewGameData;

    private void OnEnable()
    {
        ChooseName_GUI_Manager.OnPlayerNameConfirmed += OpenConfirmProfileGUI;
        ChooseImage_GUI_Manager.OnImageChoosen += UpdateProfilImageButton;
    }

    private void OnDisable()
    {
        ChooseName_GUI_Manager.OnPlayerNameConfirmed -= OpenConfirmProfileGUI;
        ChooseImage_GUI_Manager.OnImageChoosen -= UpdateProfilImageButton;
    }   

    void IDataPersistance.SaveGameData(ref GameData gameData)
    {

        if (!string.IsNullOrEmpty(_enteredName))
        {
            if(_overrideNewGameData)
            {
                gameData.PlayerName = _enteredName;
                gameData.PlayerProfileSprite = _enteredImage.name;
                gameData.ProfileIsCreated = true;

                gameData.TutorialIsCompleted = false;

                foreach(var item in gameData.LevelHighScoreList)
                {
                    item.Score = 0;
                    item.LevelCompleted = false;
                }
                
            }

            
        }
            
    }


    private void Awake()
    {
        _overrideNewGameData = false;
        _canvas = GetComponent<Canvas>();
        _initialSpriteProfilImageButton = _profilImageButton.gameObject.GetComponent<Image>().sprite;
        _canvas.enabled = false;
        _profileImage = _profilImageButton.GetComponent<Image>();        
    }

    private void Start()
    {
        _confirmButton.onClick.AddListener(SaveProfileInformation);
        _returnButton.onClick.AddListener(ReturnToChooseName);
        _profilImageButton.onClick.AddListener(ChooseImageButonPressed);
        _boardMeshRenderer.enabled = false;
        _canvas.enabled = false;
    }

    private void OpenConfirmProfileGUI(string playerName)
    {
        this._playerName.text = playerName;
        _canvas.enabled = true;
        _boardMeshRenderer.enabled = true;
        Vector3 directionToCamera = Camera.main.transform.position - _board.transform.position;

        directionToCamera.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

        _board.transform.rotation = targetRotation;

        _board.transform.Rotate(-90, 0, 0);
    }

    private void ReturnToChooseName()
    {
        _canvas.enabled = false;
        _boardMeshRenderer.enabled = false;

        // Notify: ChooseName_GUI_Manager.cs to show choose name GUI
        OnReturnToChooseName.Invoke();

        ResetProfilImageButton();
        _playerName.text = null;
    }

    private void ChooseImageButonPressed()
    {
        _canvas.enabled = false;
        _boardMeshRenderer.enabled = false;

        // Notify: ChooseImage_GUI_Manager.cs to show choose image GUI
        OnChooseImageButtonPressed.Invoke();
    }

    private void UpdateProfilImageButton(Sprite sprite)
    {
        _profilImageButton.image.sprite = sprite;
        _profileImage.color = Color.white;
        Color normalColor = new Color(159.0f / 255.0f, 159.0f / 255.0f, 159.0f / 255.0f, 1.0f);
        ColorBlock colorBlock = _profilImageButton.colors;
        colorBlock.normalColor = normalColor;
        colorBlock.highlightedColor = Color.white;
        _profilImageButton.colors = colorBlock;
    }

    private void ResetProfilImageButton()
    {
        _profilImageButton.image.sprite = _initialSpriteProfilImageButton;
    }

    // Called from ConfrimButton if user confirms his picture and name
    private void SaveProfileInformation()
    {
        if (_playerName.text != null && _profilImageButton.image.sprite != null)
        {
            _enteredName = _playerName.text;
            _enteredImage = _profilImageButton.image.sprite;
            _overrideNewGameData = true;
            DataPersistanceManager.Instance.SaveGame();
            _overrideNewGameData = false;
            SceneManager.LoadScene("LoadingSCreen_01");

        }
    }

    private void ClearPlayerName(string name)
    {
        _playerName.text = null;
    }


}

