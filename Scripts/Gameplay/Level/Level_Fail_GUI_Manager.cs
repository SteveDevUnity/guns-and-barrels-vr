using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Level_Fail_GUI_Manager : MonoBehaviour
{
   
    [SerializeField] GameObject _levelPlatform;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] Canvas _canvas;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] AudioClip _audioClip;
    [SerializeField] Button _restartButton;
    [SerializeField] Button _cancelButton;

    private AudioSource _audioSource;
    private MeshRenderer _meshRenderer;
    private float _fadeInTime = 2.0f;

    private bool _canvasIsEnabled;

    public static event Action OnRestartClicked;
    public static event Action OnCancelButtonClicked;

    private void OnEnable()
    {
        Level_Manager.OnLevelCompleted += ShowLevelFailedGUIWithDelay;
        Start_Platform_Manager.OnUserOnStartplatformChanged += CloseOnlyGUI;
    }

    private void OnDisable()
    {
        Level_Manager.OnLevelCompleted -= ShowLevelFailedGUIWithDelay;
        Start_Platform_Manager.OnUserOnStartplatformChanged -= CloseOnlyGUI;
    }


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _canvas.enabled = false;        
    }

   

    private void Start()
    {
        _meshRenderer.enabled = false;
        _canvasIsEnabled = false;
        _cancelButton.onClick.AddListener(CloseGUI);
        _restartButton.onClick.AddListener(RestartLevel);
    }

    public void ShowLevelFailedGUIWithDelay(bool successfull, int endScore, int maxScore, int highScore)
    {
        if (successfull)
        {
            return;
        }

        else
        {
            StartCoroutine(DelayShowGUI(successfull, endScore, maxScore, highScore));   
        }
       
    }

    private IEnumerator DelayShowGUI(bool successfull, int endScore, int maxScore, int highScore)
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 offset = _levelPlatform.transform.forward * 3.0f + new Vector3(0, 1.5f, 0);

        transform.position = _levelPlatform.transform.position + offset;
        _scoreText.text = "Score: " + endScore;
        _canvas.enabled = true;
        _canvasIsEnabled = true;
        StartCoroutine(FadeInGUI());
        _meshRenderer.enabled = true;
        _audioSource.PlayOneShot(_audioClip, 0.3f);

    }

    // Fades in the Level_Failed_GUI
    private IEnumerator FadeInGUI()
    {
        float elapsedTime = 0.0f;
        float startAlpha = _canvasGroup.alpha;

        while (elapsedTime < _fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 1.0f, elapsedTime / _fadeInTime);
             
            yield return null;
        }

        _canvasGroup.alpha = 1.0f;
    }

    private void CloseOnlyGUI(bool userEnteredPlatform)
    {
        if (_canvasIsEnabled && !userEnteredPlatform)
        {
            _canvas.enabled = false;
            _meshRenderer.enabled = false;
        }

        else
        {
            return;
        }
    }


    private void CloseGUI()
    {      
        _canvas.enabled = false;
        _meshRenderer.enabled = false;
        _canvasIsEnabled = false;   

        //Notify: Level_Select_GUI_Manager.cs to show last level GUI
        OnCancelButtonClicked.Invoke();
    }

    private void RestartLevel()
    {
        _canvas.enabled = false;
        _meshRenderer.enabled = false;
        _canvasIsEnabled = false;

        //Notify: Countdown_Manager.cs to restart level countdown and then restart same level
        OnRestartClicked.Invoke();
    }

    
}
