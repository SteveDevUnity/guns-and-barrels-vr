using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level_Run_Timer_GUI_Manager : MonoBehaviour
{
    [SerializeField] GameObject _levelStartPlatform;
    [SerializeField] TextMeshProUGUI _levelTimerField;
    [SerializeField] TextMeshProUGUI _levelTargetsField;
    [SerializeField] Canvas _canvas;
    [SerializeField] CanvasGroup _levelCanvasGroup;
    [SerializeField] MeshRenderer _targetsBoardMeshRenderer;
    [SerializeField] MeshRenderer _timeBoardMeshRenderer;

    private AudioSource _audioSource;

    private float _elapsedTime;
    private float _tickTime = 5.0f;
    private float _fadeDuration = 2.0f;    
    private bool _gameIsPausing;
    private float _timeToShow;

    private int _totalLevelTime;

    public static float TimeLeft;
    public static int CurrentLevelScore;
    public static int RemainingTargets;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Level_Manager.OnLevelPaused += PauseGame;
        Level_Manager.OnLevelResumed += ResumeGame;
        PauseMenu_Manager.OnPauseMenuOpened += PauseGame;
        PauseMenu_Manager.OnPauseMenuResumed += ResumeGame;
        PauseMenu_Manager.OnLevelRestartClicked += RestartLevel;
        Level_Manager.OnLevelStarted += PrepareLevel_GUI;
        Level_Manager.OnLevelCompleted += CloseLevel_GUI;
        TargetBehaviour.OnNumberOfTargetsChanged += CalculateRemainingTargets;
        
    }

    private void OnDisable()
    {
        PauseMenu_Manager.OnPauseMenuOpened -= PauseGame;
        PauseMenu_Manager.OnPauseMenuResumed -= ResumeGame;
        Level_Manager.OnLevelPaused -= PauseGame;
        Level_Manager.OnLevelResumed -= ResumeGame;
        PauseMenu_Manager.OnLevelRestartClicked -= RestartLevel;
        Level_Manager.OnLevelStarted -= PrepareLevel_GUI;
        Level_Manager.OnLevelCompleted -= CloseLevel_GUI;
        TargetBehaviour.OnNumberOfTargetsChanged -= CalculateRemainingTargets;

    }

    private void Start()
    {
        _targetsBoardMeshRenderer.enabled = false;
        _timeBoardMeshRenderer.enabled = false;
        _canvas.enabled = false;    
        CurrentLevelScore = 0;
        _gameIsPausing = true;
    }


    private void PauseGame()
    {
        _gameIsPausing = true;
    }

    private void ResumeGame()
    {
        _gameIsPausing = false;
    }

    private void Update()
    {
        // CurrentLevelScore is calculated by TargetBehaviour.cs everytime player hits a target

        if (!_gameIsPausing)
        {
            _elapsedTime += Time.deltaTime;

            TimeLeft = _totalLevelTime - _elapsedTime;

            _timeToShow = (_totalLevelTime + 1.0f) - _elapsedTime;

            // Caclculating and Showing of current Level-Time and Level-Score
            int minutes = Mathf.FloorToInt(_timeToShow / 60);
            int seconds = Mathf.FloorToInt(_timeToShow % 60);

            if (TimeLeft <= _tickTime && _tickTime >= 0)
            {
                _tickTime -= 1.0f;
                _audioSource.Play();
                _levelTimerField.color = Color.magenta;
            }

            if (TimeLeft <= 0)
            {
                Level_Manager.LevelTimeisUp = true;

                _levelTimerField.text = "Time: " + string.Format("{0:00}:{1:00}", 0, 0);
                _levelTargetsField.text = "Targets: " + RemainingTargets;
            }

            else
            {
                _levelTimerField.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
                _levelTargetsField.text = "Targets: " + RemainingTargets;
            }

        }

        else
        {
            return;
        }
    }

    private void PrepareLevel_GUI(int totalTime)
    {
        CalculatePosition();
        _levelCanvasGroup.alpha = 1.0f;
        _canvas.enabled = true;
        _totalLevelTime = totalTime;
        _elapsedTime = 0.0f;
        _targetsBoardMeshRenderer.enabled = true;
        _timeBoardMeshRenderer.enabled = true;
        _gameIsPausing = false;
             
    }

    private void RestartLevel()
    {
        _levelCanvasGroup.alpha = 0.0f;
        _levelTimerField.color = Color.white;
        TimeLeft = 0.0f;
        CurrentLevelScore = 0;
        _canvas.enabled = false;
        _elapsedTime = 0.0f;
        _tickTime = 5.0f;
        _targetsBoardMeshRenderer.enabled = false;
        _timeBoardMeshRenderer.enabled = false;

    }
 
    private void CloseLevel_GUI(bool successful, int endScore, int maxScore, int highScore)
    {
        _gameIsPausing = true;
        _elapsedTime = 0.0f;
               
        StartCoroutine(FadingOutCanvas());

        CurrentLevelScore = 0;
        _elapsedTime = 0.0f;
        _tickTime = 5.0f;       
    }

    // Fading out the Level_Run_GUI
    private IEnumerator FadingOutCanvas()
    {
        float startAlpha = _levelCanvasGroup.alpha;

        float timeElapsed = 0.0f;

        while(timeElapsed < _fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            _levelCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, timeElapsed / _fadeDuration);

            yield return null;
        }

        _levelCanvasGroup.alpha = 0.0f;
        _levelTimerField.color = Color.white;
        TimeLeft = 0.0f;
        _canvas.enabled = false;
        _targetsBoardMeshRenderer.enabled = false;
        _timeBoardMeshRenderer.enabled = false;
    }

    private void CalculatePosition()
    {
        Vector3 offset = _levelStartPlatform.transform.forward * 1.2f + new Vector3 (0.0f, 0.3f, 0.0f);
        transform.position = _levelStartPlatform.transform.position + offset;
    }

    private void CalculateRemainingTargets()
    {
        if(RemainingTargets >  0)
        {
            RemainingTargets--;
        }
    }
    

}
