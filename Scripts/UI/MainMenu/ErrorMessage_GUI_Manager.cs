using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ErrorMessage_GUI_Manager : MonoBehaviour
{
    [SerializeField] Button _confirmButton;
    [SerializeField] MeshRenderer _boardMeshRenderer;
    [SerializeField] GameObject _board;
    
    private LocalizeStringEvent _localizeStringEvent;
    
    private Canvas canvas;

    public static event Action OnErrorMessageConfirmed;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        _localizeStringEvent = GetComponentInChildren<LocalizeStringEvent>();
    }


    private void OnEnable()
    {
        ChooseName_GUI_Manager.OnErrorInputField += OpenCanvas;
    }

    private void OnDisable()
    {
        ChooseName_GUI_Manager.OnErrorInputField -= OpenCanvas;
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas.enabled = false;
        _boardMeshRenderer.enabled = false;
        _confirmButton.onClick.AddListener(ErrorConfirmed);
        
    }


    private void OpenCanvas(int errorIndex)
    {
        switch (errorIndex)
        {
            case 0: _localizeStringEvent.StringReference.SetReference("MyTexts", "message_error_01");
                break;
            case 1: _localizeStringEvent.StringReference.SetReference("MyTexts", "message_error_02");
                break;
        }

        canvas.enabled = true;
        _boardMeshRenderer.enabled = true;
        Vector3 directionToCamera = Camera.main.transform.position - _board.transform.position;

        directionToCamera.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

        _board.transform.rotation = targetRotation;

        _board.transform.Rotate(-90, 0, 0);

    }

    private void ErrorConfirmed()
    {
        // Notify: ChooseName_GUI_Manager.cs to restore default choose name GUI
        OnErrorMessageConfirmed.Invoke();

        canvas.enabled = false;
        _boardMeshRenderer.enabled = false;
    }
    



}
