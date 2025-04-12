using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Countdown_Manager : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI _counter;
    [SerializeField] TextMeshProUGUI _headline;
    [SerializeField] GameObject _level_Platform;  
    private Canvas m_Canvas;

    private float _elapsedTime;  
    private bool _timerStarted;

    public static event Action OnTimerFinished;
    public static event Action OnCountDownCanceled;
  
    private void Awake()
    {
        m_Canvas = GetComponent<Canvas>();
        m_Canvas.enabled = false;    
    }

    private void OnEnable()
    {
        Level_Select_GUI_Manager.OnLevelSelected += PlayStartCountdown;
        PauseMenu_Manager.OnLevelRestartClicked += PlayStartCountdown;
        Level_Fail_GUI_Manager.OnRestartClicked += PlayStartCountdown;
        Start_Platform_Manager.OnUserOnStartplatformChanged += HandleUserOnPlatformChange;
    }

    private void OnDisable()
    {
        Level_Select_GUI_Manager.OnLevelSelected -= PlayStartCountdown;
        PauseMenu_Manager.OnLevelRestartClicked -= PlayStartCountdown;
        Level_Fail_GUI_Manager.OnRestartClicked -= PlayStartCountdown;
        Start_Platform_Manager.OnUserOnStartplatformChanged -= HandleUserOnPlatformChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        _timerStarted = false;       
    }

    // Update is called once per frame
    void Update()
    {
        if(_timerStarted)
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime < 0.8f)
            {   
                _headline.text = "Get Ready!";                
            }

            if (_elapsedTime >= 0.8f)
            {             
                _headline.text = null;                
            }

            if (_elapsedTime >= 1.0f && _elapsedTime <= 2.0f)
            {  
                _counter.text = "3";       
            }
            
            else if (_elapsedTime >= 2.0f && _elapsedTime <= 3.0f)
            {           
                _counter.text = "2";
            }

            else if (_elapsedTime >= 3.0f && _elapsedTime <= 4.0f)
            {                   
                _counter.text = "1";
            }


      
            else if(_elapsedTime >= 4.0f)
            {
                _timerStarted = false;               
                m_Canvas.enabled = false;

                //Notify: LevelManager.cs to initialize next level
                OnTimerFinished.Invoke();
            }
      
        }

    }

    
    public void PlayStartCountdown()
    {
        transform.position = _level_Platform.transform.forward * 12.0f;
        transform.rotation = _level_Platform.transform.rotation;
        _elapsedTime = 0.0f;
        _headline.text = null;
        _counter.text = null;
        m_Canvas.enabled = true;
        _timerStarted = true;
        
    }

    // If Player had contact with StartPlatform and then left before Startcountdown is expired, StartLevel Countdown will reset,
    // and StartLevel_GUI will disappear. Will get called with UnityEvent UserLeftPlatform from Start_Platform_Manager.Cs
    public void StopStartCountdown()
    {
        _timerStarted = false; 
        _headline.text = null;

        _counter.text = null;
        m_Canvas.enabled = false;
    }

    private void HandleUserOnPlatformChange(bool userIsOnPlatform)
    {
        if (_timerStarted && !userIsOnPlatform)
        {
            StopStartCountdown();

            //Notify: CountdownCancelManger.cs to show warning message to player
            OnCountDownCanceled.Invoke();
        }

    }








}
