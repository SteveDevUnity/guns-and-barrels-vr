using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CountdownCancel_GUI_Manager : MonoBehaviour
{
    [SerializeField] GameObject level_Platform;
    
    private Canvas _canvas;

    private void OnEnable()
    {
        Countdown_Manager.OnCountDownCanceled += ShowWarningWithDelay;
    }

    private void OnDisable()
    {
        Countdown_Manager.OnCountDownCanceled -= ShowWarningWithDelay;
    }

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
    }


    public void ShowWarningMessage()
    {
        transform.position = level_Platform.transform.forward * 12.0f;
        transform.rotation = level_Platform.transform.rotation;
      
        _canvas.enabled = true;
    }

    
    public void CloseWarningMessage()
    {
        _canvas.enabled = false;
    }

    private IEnumerator WarningDelay()
    {
        yield return new WaitForSeconds(0.3f);
        ShowWarningMessage();
        yield return new WaitForSeconds(3.0f);
        CloseWarningMessage();
    }

    private void ShowWarningWithDelay()
    {
        StartCoroutine(WarningDelay());
    }
    

}
