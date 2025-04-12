using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Level_End_GUI_Manager : MonoBehaviour
{

    [SerializeField] GameObject _levelPlatform;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _highScoreText;
    [SerializeField] UnityEngine.UI.Button _okButton;
    [SerializeField] Canvas _canvas;
    [SerializeField] AudioClip _audioClip;

    private AudioSource _audioSource;

    public static int currentLevel_CompletedScore;
    public static float currentLevel_CompletedTime;

    private float _lerpTimeSize = 2.0f;
    private bool _isBlinking;
    private MeshRenderer _meshRenderer;
    
    private int _highScore;
    private bool _canvasIsEnabled;

    public static event Action OnOkButtonClicked;


    private void OnEnable()
    {
        Level_Manager.OnLevelCompleted += ShowLvlCompletedGuiWithDelay;
        Start_Platform_Manager.OnUserOnStartplatformChanged += CloseOnlyGUI;
    }

    private void OnDisable()
    {
        Level_Manager.OnLevelCompleted -= ShowLvlCompletedGuiWithDelay;
        Start_Platform_Manager.OnUserOnStartplatformChanged -= CloseOnlyGUI;
    }


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _canvas.enabled = false;
        _meshRenderer = GetComponent<MeshRenderer>();      
        
    }


    // Start is called before the first frame update
    void Start()
    {
        _canvasIsEnabled = false;
        _isBlinking = false;
        _meshRenderer.enabled = false;
        _okButton.onClick.AddListener(CloseGUI);
        
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
        OnOkButtonClicked.Invoke();
    }


    public void ShowLvlCompletedGuiWithDelay(bool successful, int endScore, int maxScore, int highScore)
    {

        if (!successful)
        {
            return;
        }

        else 
        {
            StartCoroutine(DelayToShowGUI(successful, endScore, maxScore, highScore));  
        }

    }


    private IEnumerator DelayToShowGUI(bool successful, int endScore, int maxScore, int highScore)
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 offset = _levelPlatform.transform.forward * 3.0f + new Vector3(0, 1.5f, 0);
        transform.position = _levelPlatform.transform.position + offset;

        _scoreText.text = "Score: " + endScore;

        _highScoreText.text = "HighScore: " + highScore;
        _audioSource.PlayOneShot(_audioClip, 0.3f);
        _canvas.enabled = true;
        StartCoroutine(ScalingSize());
        StartCoroutine(FadingIn());
        _meshRenderer.enabled = true;
        _canvasIsEnabled = true;
    }

    // Scale the score text over time
    private IEnumerator ScalingSize()
    {
        float scaleStart = _scoreText.fontSize;
        float scaleEnd = 0.4f;

        

        float elapsedTime = 0;

        while(elapsedTime < _lerpTimeSize)
        {
            elapsedTime += Time.deltaTime;

            _scoreText.fontSize = Mathf.Lerp(scaleStart, scaleEnd, elapsedTime / _lerpTimeSize);

            yield return null;
        }

        _scoreText.fontSize = 0.4f;
        _isBlinking = true;
        

        // score text is blinking a few times
        while(_isBlinking)
        {
            _scoreText.alpha = 0.0f;
            yield return new WaitForSeconds(0.2f);
            _scoreText.alpha = 1.0f;
            yield return new WaitForSeconds(0.2f);
            _scoreText.alpha = 0.0f;
            yield return new WaitForSeconds(0.2f);
            _scoreText.alpha = 1.0f;
            yield return new WaitForSeconds(0.2f);
            _scoreText.alpha = 0.0f;
            yield return new WaitForSeconds(0.2f);
            _scoreText.alpha = 1.0f;
            yield return new WaitForSeconds(0.2f);

            _isBlinking = false;
        }
     
    }


    // Fading in the score text over time
    private IEnumerator FadingIn()
    {
        float startAlpha = 0.0f;
        float endAlpha = 1.0f;

        float elapsedTime = 0;

        while (elapsedTime < _lerpTimeSize)
        {
            elapsedTime += Time.deltaTime;

            _scoreText.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / _lerpTimeSize);
            yield return null;
        }

        _scoreText.alpha = 1.0f;
    }
}
