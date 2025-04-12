using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour, IDataPersistance
{
    [SerializeField] CanvasGroup _startTutorialGroup;
    [SerializeField] CanvasGroup _gripTutorialGroup;
    [SerializeField] CanvasGroup _triggerTutorialGroup;
    [SerializeField] CanvasGroup _menuTutorialGroup;
    [SerializeField] CanvasGroup _endTutorialGroup;

    [SerializeField] Button _startTutorialButton;
    [SerializeField] Button _menuTutorialButton;
    [SerializeField] Button _gripTutorialButton;
    [SerializeField] Button _triggerTutorialButton;
    [SerializeField] Button _endTutorialButton;

    [SerializeField] GameObject _tutorialArrow;
    [SerializeField] GameObject _controllerRight;
    [SerializeField] GameObject _controllerLeft;

    [SerializeField] AudioClip _hintSound;
    [SerializeField] AudioClip _taskSound;
    
    [SerializeField] TextMeshProUGUI _gripHeaderText;
    [SerializeField] TextMeshProUGUI _triggerHeaderText;
    [SerializeField] TextMeshProUGUI _gripMessageText;
    [SerializeField] TextMeshProUGUI _triggerMessageText;

    private bool _triggerMenuFirstPressed;
    private bool _triggerMenuSecondPressed;
    private bool _gripMenuFirstPressed;

    private bool _tutorialIsCompleted;
    private Animator _animator;
    private MeshRenderer _arrowMeshRenderer;

    private Color32 _colorBtnEnabled = new Color32(27, 183, 15, 255);
    private Color32 _colorBtnDisabled = new Color32(65, 65, 65, 255);
    private AudioSource _audioSource;

    public static event Action OnGripTutorialConfirmed;
    public static event Action OnTutorialCompleted;

    private void Awake()
    {
        _arrowMeshRenderer = _tutorialArrow.GetComponent<MeshRenderer>();

        _startTutorialButton.enabled = false;
        _audioSource = GetComponent<AudioSource>();

        _endTutorialButton.enabled = false;
        _gripTutorialButton.enabled = false;
        _triggerTutorialButton.enabled = false;
        _menuTutorialButton.enabled = false;

        _endTutorialButton.interactable = false;

        _startTutorialButton.interactable = false;
        _gripTutorialButton.interactable = false;
        _triggerTutorialButton.interactable = false;
        _menuTutorialButton.interactable = false;

        _startTutorialGroup.alpha = 0;
        _gripTutorialGroup.alpha = 0;
        _triggerTutorialGroup.alpha = 0;

        _menuTutorialGroup.alpha = 0;
        _endTutorialGroup.alpha = 0;

        _startTutorialGroup.interactable = false;
        _gripTutorialGroup.interactable = false;
        _triggerTutorialGroup.interactable = false;

        _menuTutorialGroup.interactable = false;
        _endTutorialGroup.interactable = false;

        _startTutorialGroup.blocksRaycasts = false;
        _gripTutorialGroup.blocksRaycasts = false;
        _triggerTutorialGroup.blocksRaycasts = false;

        _menuTutorialGroup.blocksRaycasts = false;
        _endTutorialGroup.blocksRaycasts = false;

        _animator = GetComponent<Animator>();

        _controllerRight.SetActive(false);
        _controllerLeft.SetActive(false);

        _arrowMeshRenderer.enabled = false;

        _gripTutorialButton.image.color = _colorBtnDisabled;
        _triggerTutorialButton.image.color = _colorBtnDisabled;
        _menuTutorialButton.image.color = _colorBtnDisabled;
    }

    private void OnEnable()
    {
        TutorialBottleManager.OnBottleShot += SaveTutorialCompleted;
        TutorialBottleManager.OnBottleShot += ShowMenuTutorial;
        Colt_Manager.OnWeaponGrabbedFirstTime += ShowTriggerTutorial;
        DataPersistanceManager.OnLoadingCompleted += InitializeAfterLoad;
    }


    private void OnDisable()
    {
        TutorialBottleManager.OnBottleShot -= SaveTutorialCompleted;
        TutorialBottleManager.OnBottleShot -= ShowMenuTutorial;
        Colt_Manager.OnWeaponGrabbedFirstTime -= ShowTriggerTutorial;
        DataPersistanceManager.OnLoadingCompleted -= InitializeAfterLoad;
    }

    void IDataPersistance.LoadGameData(GameData gameData)
    {
        _tutorialIsCompleted = gameData.TutorialIsCompleted;

        if (_tutorialIsCompleted)
        {
            TutorialBottleManager.BottleIsDestructable = true;
        }
    }

    void IDataPersistance.SaveGameData(ref GameData gameData)
    {
        gameData.TutorialIsCompleted = _tutorialIsCompleted;
    }


    private void InitializeAfterLoad()
    {
            if (!_tutorialIsCompleted)
            {
                _startTutorialButton.onClick.AddListener(OnStartConfirmed);
                _endTutorialButton.onClick.AddListener(OnEndConfirmed);
                _menuTutorialButton.onClick.AddListener(OnMenuConfirmed);
                _triggerTutorialButton.onClick.AddListener(OnTriggerConfirmed);
                _gripTutorialButton.onClick.AddListener(OnGripConfirmed);

                StartCoroutine(DelayTutorialStart());
                _triggerMenuFirstPressed = false;
                _triggerMenuSecondPressed = false;
            }

            else
            {
                Destroy(gameObject);
            }
    }

    private IEnumerator DelayTutorialStart()
    {
        yield return new WaitForSeconds(0.6f);
        StartTutorial();
    }

    private IEnumerator DelayShowPanel(CanvasGroup panelGroup, Button panelButton)
    {
        yield return new WaitForSeconds(0.3f);
        _audioSource.PlayOneShot(_hintSound);
        ShowPanel(panelGroup);
    }

    private void StartTutorial()
    {
        _audioSource.PlayOneShot(_hintSound);
        ShowPanel(_startTutorialGroup);
        _startTutorialButton.enabled = true;
        _startTutorialButton.interactable = true;
    }

    private void ShowPanel(CanvasGroup panelGroup)
    {
        panelGroup.alpha = 1;
        panelGroup.interactable = true;
        panelGroup.blocksRaycasts = true;
        
    }

    private void HidePanel(CanvasGroup panelGroup, Button panelButton)
    {
        panelGroup.alpha = 0;
        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;
        panelButton.enabled = false;
        panelButton.interactable = false;
        _arrowMeshRenderer.enabled = false;
    }

    private void OnStartConfirmed()
    {
        HidePanel(_startTutorialGroup, _startTutorialButton);

        StartCoroutine(DelayShowPanel(_gripTutorialGroup, _gripTutorialButton));

        _gripHeaderText.text = LocalizationSettings.StringDatabase.GetLocalizedString("MyTexts", "hint_h1");
        _gripMessageText.text = LocalizationSettings.StringDatabase.GetLocalizedString("MyTexts",
            "tutorial_grip_message");

        StartCoroutine(DelayAnimation("OnStartGrid", _controllerRight));
    }

    private void OnGripConfirmed()
    {
        if (!_gripMenuFirstPressed)
        {
            _arrowMeshRenderer.enabled = false;
            _gripMenuFirstPressed = true;
            _arrowMeshRenderer.enabled = false;
            _controllerRight.SetActive(false);
            _gripHeaderText.text = LocalizationSettings.StringDatabase.GetLocalizedString("MyTexts",
                "task_h1");
            _gripHeaderText.color = new Color32(135, 8, 87, 255);
            _gripMessageText.text = LocalizationSettings.StringDatabase.GetLocalizedString("MyTexts",
                "tutorial_task_findWeapon");

            _audioSource.PlayOneShot(_taskSound);
        }

        else
        {
            HidePanel(_gripTutorialGroup, _gripTutorialButton);

            // Notify:
            // TutorialDrawerManger.cs to enable opening drawer
            // ColtManager.cs to make colt weapon grabbable
            OnGripTutorialConfirmed.Invoke();
        }

    }

    private void OnTriggerConfirmed()
    {
        if (!_triggerMenuFirstPressed)
        {
            _triggerTutorialButton.image.color = _colorBtnDisabled;
            _triggerTutorialButton.enabled = false;
            _triggerTutorialButton.interactable = false;

            _triggerMenuFirstPressed = true;

            _triggerHeaderText.text = LocalizationSettings.StringDatabase.GetLocalizedString("MyTexts",
            "hint_h1");
            _triggerMessageText.text = LocalizationSettings.StringDatabase.GetLocalizedString("MyTexts",
                "tutorial_trigger_message");

            StartCoroutine(DelayAnimation("OnStartTrigger", _controllerRight));
            _audioSource.PlayOneShot(_hintSound);
        }

        else if (_triggerMenuFirstPressed && !_triggerMenuSecondPressed)
        {
            _triggerMenuSecondPressed = true;
            _arrowMeshRenderer.enabled = false;
            _controllerRight.SetActive(false);

            _triggerHeaderText.text = LocalizationSettings.StringDatabase.GetLocalizedString("MyTexts",
            "task_h1");
            _triggerHeaderText.color = new Color32(135, 8, 87, 255);
            _triggerMessageText.text = LocalizationSettings.StringDatabase.GetLocalizedString("MyTexts",
                "tutorial_task_shootBottle");

            _audioSource.PlayOneShot(_taskSound);

        }

        else if (_triggerMenuFirstPressed && _triggerMenuSecondPressed)
        {
            HidePanel(_triggerTutorialGroup, _triggerTutorialButton);
            TutorialBottleManager.BottleIsDestructable = true;
        }

    }

    private void OnMenuConfirmed()
    {
        HidePanel(_menuTutorialGroup, _menuTutorialButton);
        _controllerLeft.SetActive(false);
        _arrowMeshRenderer.enabled = false;
        ShowPanel(_endTutorialGroup);
        _endTutorialButton.enabled = true;
        _endTutorialButton.interactable = true;
    }

    private void OnEndConfirmed()
    {
        _tutorialIsCompleted = true;
        DataPersistanceManager.Instance.SaveGame();

        //Notify: TutorialDoormanager.cs that tutorial is completed
        OnTutorialCompleted.Invoke();

        PlayerPosition.PlayerBeenMoved = true;
        Destroy(gameObject);
    }

    private void SaveTutorialCompleted()
    {
        _tutorialIsCompleted = true;
        DataPersistanceManager.Instance.SaveGame();
    }

    private void ShowTriggerTutorial()
    {
        StartCoroutine(DelayShowPanel(_triggerTutorialGroup, _triggerTutorialButton));

        _triggerHeaderText.text = LocalizationSettings.StringDatabase.GetLocalizedString("MyTexts",
            "hint_h1");
        _triggerMessageText.text = LocalizationSettings.StringDatabase.GetLocalizedString("MyTexts",
            "tutorial_snapWeapon_message");
        _triggerTutorialButton.image.color = _colorBtnEnabled;
        _triggerTutorialButton.enabled = true;
        _triggerTutorialButton.interactable = true;

    }

    private void ShowMenuTutorial()
    {
        StartCoroutine(DelayShowPanel(_menuTutorialGroup, _menuTutorialButton));
        StartCoroutine(DelayAnimation("OnStartMenu", _controllerLeft));
    }

    private IEnumerator DelayAnimation(string triggerName, GameObject Controller)
    {
        yield return new WaitForSeconds(1.0f);
        Controller.SetActive(true);
        _animator.SetTrigger(triggerName);
    }

    public void ShowArrow()
    {
        _arrowMeshRenderer.enabled = true;
    }


    // Called by Animation Grip_Button
    public void EnableTutorialGripButton()
    {
        _gripTutorialButton.enabled = true;
        _gripTutorialButton.interactable = true;

        _gripTutorialButton.image.color = _colorBtnEnabled;

    }

    // Called by Animation Trigger_Button
    public void EnableTutorialTriggerButton()
    {
        _triggerTutorialButton.enabled = true;
        _triggerTutorialButton.interactable = true;

        _triggerTutorialButton.image.color = _colorBtnEnabled;
    }

    // Called by Animation Menu_Button
    public void EnableTutorialMenuButton()
    {
        _menuTutorialButton.enabled = true;
        _menuTutorialButton.interactable = true;

        _menuTutorialButton.image.color = _colorBtnEnabled;
    }
}

