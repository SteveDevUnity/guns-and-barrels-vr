using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Playables;

public class OptionsMenu_GUI_Manager : MonoBehaviour, IDataPersistance
{
    [SerializeField] Button _confirmButton;
    [SerializeField] Button _cancelButton;
    [SerializeField] TMP_Dropdown _dropDownMenu;
    [SerializeField] TextMeshProUGUI _labelText;
    [SerializeField] MeshRenderer _boardMeshRenderer;
    [SerializeField] GameObject _board;

    private Canvas _canvas;
    private int _dropdownValue;
    private int _localIndex;
    
    public static event Action OnOptionsGUIClosed;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();    
    }

    private void OnEnable()
    {        
        MainMenu_GUI_Manager.OnOptionsBoardClicked += OpenOptionsGui;
        DataPersistanceManager.OnCheckProfileIsCreated += SetLanguageFirstTime;
    }

    private void OnDisable()
    {        
        MainMenu_GUI_Manager.OnOptionsBoardClicked -= OpenOptionsGui;
        DataPersistanceManager.OnCheckProfileIsCreated -= SetLanguageFirstTime;
    }   

    void IDataPersistance.LoadGameData(GameData gameData)
    {
        _localIndex = gameData.LocalIndex;
        SetLanguage();
    }

    void IDataPersistance.SaveGameData(ref GameData gameData)
    {
        gameData.LocalIndex = _localIndex;
    }

    void Start()
    {       
        _dropDownMenu.onValueChanged.AddListener(OnLanguageChanged);
        _confirmButton.onClick.AddListener(ConfirmOptions);
        _cancelButton.onClick.AddListener(CloseOptionsGUI);
        _boardMeshRenderer.enabled = false;
        _canvas.enabled = false;
        _dropDownMenu.enabled = false;
    }

    // User selected an language option from the dropdownlist
    private void OnLanguageChanged(int dropdownValue)
    {
        _dropdownValue = dropdownValue;
    }


    // Called when user presses the confirm button in the optionsmenu, language changed
    private void ConfirmOptions()
    {
        _localIndex = _dropdownValue;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localIndex];
        DataPersistanceManager.Instance.SaveGame();
        CloseOptionsGUI();
    }

    private void CloseOptionsGUI()
    {
        _canvas.enabled = false;
        _boardMeshRenderer.enabled = false;

        // Notify: MainMenu_GUI_Manager.cs to show mainmenu sign
        OnOptionsGUIClosed.Invoke();

        _dropDownMenu.enabled = false;
    }

    private void OpenOptionsGui()
    {
        _dropDownMenu.onValueChanged.RemoveListener(OnLanguageChanged);
        _dropDownMenu.value = _localIndex;
        _dropDownMenu.onValueChanged.AddListener(OnLanguageChanged);

        _canvas.enabled = true;
        _boardMeshRenderer.enabled = true;
        Vector3 directionToCamera = Camera.main.transform.position - _board.transform.position;

        directionToCamera.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

        _board.transform.rotation = targetRotation;

        _board.transform.Rotate(-90, 0, 0);
        _dropDownMenu.enabled = true;      
        
    }

    // Sets the saved language on every load 
    private void SetLanguage()
    {
        StartCoroutine(ChangeLocale(_localIndex));
    }

    // Sets the language when user starts the game for the very first time
    private void SetLanguageFirstTime(bool isCreated)
    {
        if (!isCreated)
        {
            DetermineSystemLanguage();
            StartCoroutine(ChangeLocale(_localIndex));
        }
        
    }

    private IEnumerator ChangeLocale(int localNumber)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localNumber];
        yield return null;
    }

    private void DetermineSystemLanguage()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;

        switch (systemLanguage)
        {
            case SystemLanguage.English:
                _localIndex = 0;
                break;
            case SystemLanguage.French:
                _localIndex = 1;
                break;
            case SystemLanguage.German:
                _localIndex = 2;
                break;
            case SystemLanguage.Spanish:
                _localIndex = 3;
                break;
            default:
                _localIndex = 0;
                break;
        }

    }


}
