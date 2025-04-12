using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PlayerPosition : MonoBehaviour, IDataPersistance
{
    private bool _tutorialIsCompleted;
    private XROrigin _xROrigin;
    public static bool PlayerBeenMoved;

    private void Awake()
    {
        _xROrigin = GetComponent<XROrigin>();
        PlayerBeenMoved = false;
    }

    private void OnEnable()
    {
        TutorialManager.OnTutorialCompleted += ToggleTutorialCompleted;
    }

    private void OnDestroy()
    {
        TutorialManager.OnTutorialCompleted -= ToggleTutorialCompleted;
    }

    void IDataPersistance.LoadGameData(GameData gameData)
    {
        _tutorialIsCompleted = gameData.TutorialIsCompleted;

        if (_tutorialIsCompleted && !PlayerBeenMoved)
        {
            Vector3 cameraOffset = Camera.main.transform.position - _xROrigin.transform.position;
            transform.position = gameData.PlayerPosition - cameraOffset;

            PlayerBeenMoved = true;
        }
    }

    void IDataPersistance.SaveGameData(ref GameData gameData)
    {
        if (_tutorialIsCompleted)
        {
            gameData.PlayerPosition = _xROrigin.Camera.transform.position;
        }             
    }


    private void ToggleTutorialCompleted()
    {
        _tutorialIsCompleted = true;
    }
}
