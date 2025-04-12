using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Level_Select_GUI_Manager : MonoBehaviour, IDataPersistance
{
    [SerializeField] TextMeshProUGUI _levelNumberText;
    [SerializeField] TextMeshProUGUI _highScoreText;
    [SerializeField] GameObject _levelStartPlatform;

    [SerializeField] Button _startButton;
    [SerializeField] Button _rightButton;
    [SerializeField] Button _leftButton;
    [SerializeField] Image _checkmarkImage;

    [SerializeField] Sprite _completedSprite;
    [SerializeField] Sprite _failedSprite;

    [SerializeField] Canvas _canvas;

    public Color32 ButtonColor_enabled;
    public Color32 ButtonColor_disabled;

    private Image _buttonImage;
    private bool _enableRightButton;
    private MeshRenderer _meshRenderer;

    private List<LevelHighScore> _levelHighScoresList;

    public static int CurrentLevelToShow;
    
    public static event Action OnLevelSelected;


    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _canvas.enabled = false;
        _buttonImage = _rightButton.GetComponent<Image>();
    }

    private void OnEnable()
    {
        Level_Manager.OnShowSelectLevelGUI += ShowLevelAtStart;
        Level_Manager.OnCloseSelectLevelGUI += CloseLevelSelectGUI;
        Level_Fail_GUI_Manager.OnCancelButtonClicked += ShowLevelAtStart;
        Level_End_GUI_Manager.OnOkButtonClicked += ShowLevelAtStart;
    }

    private void OnDisable()
    {
        Level_Manager.OnShowSelectLevelGUI -= ShowLevelAtStart;
        Level_Manager.OnCloseSelectLevelGUI -= CloseLevelSelectGUI;
        Level_Fail_GUI_Manager.OnCancelButtonClicked -= ShowLevelAtStart;
        Level_End_GUI_Manager.OnOkButtonClicked -= ShowLevelAtStart;
    }

    void IDataPersistance.LoadGameData(GameData gameData)
    {
        _levelHighScoresList = gameData.LevelHighScoreList;
    }


    private void Start()
    {
        _startButton.onClick.AddListener(StartButtonClicked);
        _leftButton.onClick.AddListener(ShowPreviousLevel);
        _rightButton.onClick.AddListener(ShowNextLevel);
        _meshRenderer.enabled = false;

    }

    private void Update()
    {
        if (CurrentLevelToShow == 1)
        {
            _leftButton.gameObject.GetComponent<Image>().enabled = false;
            _leftButton.enabled = false;           
        }

        else
        {
            _leftButton.gameObject.GetComponent<Image>().enabled = true;
            _leftButton.enabled = true;           
        }
    }

    private void StartButtonClicked()
    {
        Level_Manager.CurrentLevel = CurrentLevelToShow;
        CloseLevelSelectGUI();

        //Notify: Countdown_Manager.cs to start level countdown
        OnLevelSelected.Invoke();
        
    }

    private void ShowPreviousLevel()
    {
        CurrentLevelToShow --;
        ShowSpecificLevel(CurrentLevelToShow);
    }

    private void ShowNextLevel()
    {
        if (CurrentLevelToShow == 10)
        {
            CurrentLevelToShow = 1;
        }

        else
        {
            CurrentLevelToShow ++;
        }
        
        ShowSpecificLevel(CurrentLevelToShow);
    }


    public void ShowLevelAtStart()
    {
        DataPersistanceManager.Instance.LoadGame();

        Vector3 offset = new Vector3(0, 1.5f, 0);
        transform.position = _levelStartPlatform.transform.position + _levelStartPlatform.transform.forward * 4.0f + offset;

        LevelHighScore levelHighScore = _levelHighScoresList.FirstOrDefault(lv => lv.LevelCompleted == false);

        if(levelHighScore == null)
        {
            levelHighScore = _levelHighScoresList.FirstOrDefault(lv => lv.LevelCompleted == true);
        }

        CurrentLevelToShow = levelHighScore.Level;

        Level_Manager.HighScore = levelHighScore.Score;

        _levelNumberText.text = "Level: " + levelHighScore.Level;
        _highScoreText.text = "HighScore: " + levelHighScore.Score;

        if (levelHighScore.LevelCompleted == false)
        {
            _checkmarkImage.sprite = _failedSprite;
            _enableRightButton = false;
            
        }

        else
        {
            _checkmarkImage.sprite = _completedSprite;
            _enableRightButton = true;
        }

        
        _canvas.enabled = true;

        if (!_enableRightButton)
        {
            _rightButton.enabled = false;
            _buttonImage.color = ButtonColor_disabled;
        }

        else
        {
            _rightButton.enabled = true;
            _buttonImage.color = ButtonColor_enabled;
        }
        _meshRenderer.enabled = true;

    }


    public void ShowSpecificLevel(int currentLevelToShow)
    {
        DataPersistanceManager.Instance.LoadGame();

        LevelHighScore levelHighScore = _levelHighScoresList.Find(lv => lv.Level == currentLevelToShow);

        Level_Manager.HighScore = levelHighScore.Score;

        _levelNumberText.text = "Level: " + levelHighScore.Level;

        if (levelHighScore.LevelCompleted == false)
        {
            _checkmarkImage.sprite = _failedSprite;
           
            _enableRightButton = false;
           
        }


        else
        {
            _enableRightButton = true;
            _checkmarkImage.sprite = _completedSprite;
         
        }

        _highScoreText.text = "HighScore: " + levelHighScore.Score;
        _canvas.enabled = true;

        _buttonImage = _rightButton.GetComponent<Image>();

        if (!_enableRightButton)
        {
            _rightButton.enabled = false;
            _buttonImage.color = ButtonColor_disabled;
        }

        else
        {
            _rightButton.enabled = true;
            _buttonImage.color = ButtonColor_enabled;
        }
        _meshRenderer.enabled = true;
    }

    public void CloseLevelSelectGUI()
    {      
         _canvas.enabled = false;
        _meshRenderer.enabled = false;       
    }
}
