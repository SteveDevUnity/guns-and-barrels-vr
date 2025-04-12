using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;


public class Level_Manager : MonoBehaviour, IDataPersistance
{    
    
    [SerializeField] GameObject _levelBorder;
   
    public List<LevelData> LevelDataList = new List<LevelData>();

    private int _totalLevelTime;

    public static float LeftLevelTime;
    public static int CurrentLevelScore;
    public static int LevelMaxScore;
    public static int CurrentLevel;
    public static bool LevelIsPlaying;
    
    private int _levelEndScore;

    public static bool LevelTimeisUp;

    private List<GameObject> _remainingTargets;

    public static event Action<int> OnLevelStarted;
    public static event Action<bool, int, int, int> OnLevelCompleted;
    public static event Action<bool> OnLevelIsPlaying;
    public static event Action OnLevelPaused;
    public static event Action OnLevelResumed;
    public static event Action OnShowSelectLevelGUI;
    public static event Action OnCloseSelectLevelGUI;

    private bool _levelSuccessful;
    private int _amountOfTargets;

    public static int HighScore;
    
    private bool _gameIsPausing;   
    private bool _safeNewHighScore;
    private Vector3 _gridOrigin;
    private Vector3 _gridDimension = new Vector3(4, 2.2f, 4.5f);
    private float _cellSize = 1.5f;
    private List<Vector3> _availablePositions = new List<Vector3>();
    private List<Vector2> _usedOrthogonalPositions = new List<Vector2>();

    //************ Score System ******************************************************************************************

    // LevelMaxScore is the product from amount of Targets per Level multiplied with level specific targetData.scorePoints
    // CurrentLevelScore is 0 at each LevelStart and is increased by targetData.scorePoints in the TargetBehaviour.cs for each target hit.
    // levelEndscore is the product of the currentLevelScore and the remaining time. The more time is left, the higher the factor.
    // If the level is not completed, currentLevelScore = levelEndScore.

    // ****************************************************************************************************************************

    private void OnEnable()
    {
        Countdown_Manager.OnTimerFinished += InitializeLevel;
        PauseMenu_Manager.OnLevelRestartClicked += RestartLevel;
        Start_Platform_Manager.OnUserOnStartplatformChanged += HandleUserOnPlatformChange;
        
    }

    private void OnDisable()
    {
        Countdown_Manager.OnTimerFinished -= InitializeLevel;
        PauseMenu_Manager.OnLevelRestartClicked -= RestartLevel;
        Start_Platform_Manager.OnUserOnStartplatformChanged -= HandleUserOnPlatformChange;
    }


    // Start is called before the first frame update
    void Start()
    {
        _gridOrigin = _levelBorder.transform.position;
        DataPersistanceManager.Instance.LoadGame();
        LevelIsPlaying = false;
        LevelTimeisUp = false;
        _safeNewHighScore = false;     
    }

   

    void IDataPersistance.SaveGameData(ref GameData gameData)
    {

        if(_safeNewHighScore)
        {
            LevelHighScore levelHighScoreObject =  gameData.LevelHighScoreList.FirstOrDefault(hs => hs.Level == CurrentLevel);

            levelHighScoreObject.LevelCompleted = true;
            
            levelHighScoreObject.Score = _levelEndScore;
           
        }
        
 
    }

    private void HandleUserOnPlatformChange(bool userIsOnPlatform)
    {
        if(LevelIsPlaying && !userIsOnPlatform)
        {
            PauseLevel();
        }

        else if(LevelIsPlaying && userIsOnPlatform && _gameIsPausing)
        {
            ResumeLevel();
            
        }

        else if(!LevelIsPlaying && userIsOnPlatform)
        {
            // Notify: Level_Select_GUI_Manager.cs to show level select GUI
            OnShowSelectLevelGUI.Invoke();
        }

        else if(!LevelIsPlaying && !userIsOnPlatform)
        {
            // Notify: Level_Select_GUI_Manager.cs to hide level select GUI
            OnCloseSelectLevelGUI.Invoke();
        }

    }

    private void PauseLevel()
    {
        _gameIsPausing = true;

        // Notify: Level_Run_Timer_GUI_Manager.cs to stop timer and score updates
        // Notify: AudioPlay.cs to pause backgroundmusic
        // Notify: TargetBehaviour.cs to pause moving targets 
        OnLevelPaused.Invoke();
    }

    private void ResumeLevel()
    {
        _gameIsPausing = false;

        // Notify: Level_Run_Timer_GUI_Manager.cs to continue timer and score updates
        // Notify: AudioPlay.cs to continue backgroundmusic
        // Notify: TargetBehaviour.cs to continue moving targets
        OnLevelResumed.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (LevelIsPlaying)
        {
            // CurrentLevelScore is calculated by TargetBehaviour.cs everytime player hits a target

            // Player reached maximum Lvl Score before time is up
            if (CurrentLevelScore >= LevelMaxScore)
            {
                _levelSuccessful = true;

                OnLevelIsPlaying.Invoke(false);
                
                PauseMenu_Manager.LevelIsPlaying = false;
                LeftLevelTime = Level_Run_Timer_GUI_Manager.TimeLeft;

                _levelEndScore = CalculateCurrentLevelScore(CurrentLevelScore);

                if (_levelEndScore > HighScore)
                {
                    HighScore = _levelEndScore;
                    _safeNewHighScore = true;
                }

                OnLevelCompleted.Invoke(_levelSuccessful, _levelEndScore, LevelMaxScore, HighScore);

                DataPersistanceManager.Instance.SaveGame();
               
                LevelIsPlaying = false;
                DestroyRemainingTargets();
                _safeNewHighScore = false;

            }

            // Player didnt reach maxScore and time is up
            // levelTimeIsUp gets values from Level_Run_GUI_Manager.cs

            else if (CurrentLevelScore < LevelMaxScore && LevelTimeisUp)
            {

                _levelSuccessful = false;
                OnLevelIsPlaying.Invoke(false);
                _levelEndScore = CurrentLevelScore;

                OnLevelCompleted.Invoke(_levelSuccessful, _levelEndScore, LevelMaxScore, HighScore);

                LevelIsPlaying = false;
                PauseMenu_Manager.LevelIsPlaying = false;

                DestroyRemainingTargets();               
               
            }

        }
    }

    // Method will be called by UnityEvent TimerFinished (Timer_Manager.cs), if the StartCountdown of the LevelStart_Platform has expired.
    
    public void InitializeLevel()
    {
        _usedOrthogonalPositions.Clear();
        
        LevelData levelData = LevelDataList[CurrentLevel - 1];
        _availablePositions.Clear();
        
        GetSpawnGrid(levelData.GetMovementType());

        LevelMaxScore = 0;
        _levelEndScore = 0;
        _remainingTargets = new List<GameObject>();

        foreach (var target in levelData.TargetDataList)
        {
            for (int i = 0; i <= levelData.AmountOfTargets - 1; i++)
            {
                GameObject targetInstance = Instantiate(target.TargetPrefab, GetUniqueSpawnPositions(), target.TargetPrefab.transform.rotation);

                TargetBehaviour targetBehaviour = targetInstance.GetComponent<TargetBehaviour>();

                targetBehaviour.InitializeTarget(target, levelData.SpeedMultiplier, levelData.HealthMultiplier, target.ExplosionSound);

                targetBehaviour.SetMovementStrategy(levelData.MovementStrategy.CreateMovementStreategy());

                LevelMaxScore += target.ScorePoints;

                _remainingTargets.Add(targetInstance);
            }

        }

        _amountOfTargets = _remainingTargets.Count;
        LevelTimeisUp = false;
        Level_Run_Timer_GUI_Manager.RemainingTargets = _amountOfTargets;
        _totalLevelTime = levelData.TotalLevelTime;
        CurrentLevelScore = 0;
        OnLevelStarted.Invoke(_totalLevelTime);
        LevelIsPlaying = true;
        PauseMenu_Manager.LevelIsPlaying = true;
        OnLevelIsPlaying.Invoke(true);

    }

    // If player restarts the level from the pause menu
    public void RestartLevel()
    {
        DestroyRemainingTargets();
        LevelIsPlaying = false;
        LevelTimeisUp = false;
    }

   

    // Destroy all Remaining Targets after every Level
    public void DestroyRemainingTargets()
    {
        foreach (var target in _remainingTargets)
        {
            if (target != null)
            {
                Destroy(target);
            }
        }

        _remainingTargets.Clear();

    }


    // With current variables 60 different Positions available inside the grid 
    public void GetSpawnGrid(string movementType)
    {
        if(movementType == "Vertical")
        {
            for (float x = -4; x < _gridDimension.x; x += _cellSize)
            {
                for (float y = -2.2f; y < _gridDimension.y; y += _cellSize)
                {

                    for (float z = -4.5f; z < _gridDimension.z; z += _cellSize)
                    {

                        Vector3 position = _gridOrigin + new Vector3(x, 0, z);
                        Vector2 newOrthoPosition = new Vector2(position.x, position.z);

                        if (!_usedOrthogonalPositions.Contains(newOrthoPosition))
                        {
                            _usedOrthogonalPositions.Add(newOrthoPosition);

                            _availablePositions.Add(new Vector3(position.x,
                                Random.Range(-1.0f, 2.2f), position.z));
                        }

                    }

                }
            }
        }

        else
        {
            for (float x = -4; x < _gridDimension.x; x += _cellSize)
            {
                for (float y = -2.2f; y < _gridDimension.y; y += _cellSize)
                {

                    for (float z = -4.5f; z < _gridDimension.z; z += _cellSize)
                    {

                        Vector3 position = _gridOrigin + new Vector3(0, y, z);
                        Vector2 newOrthoPosition = new Vector2(position.y, position.z);

                        if (!_usedOrthogonalPositions.Contains(newOrthoPosition))
                        {
                            _usedOrthogonalPositions.Add(newOrthoPosition);

                            _availablePositions.Add(new Vector3(Random.Range(-4.0f, 4.0f), position.y, position.z));
                        }

                    }

                }
            }
        }
        
    }

    // Calculates unique Spawnpositions for every target
    public Vector3 GetUniqueSpawnPositions()
    {
        Vector3 randomPosition;
      
        int randomIndex = Random.Range(0, _availablePositions.Count);
       
        randomPosition = _availablePositions[randomIndex];

        _availablePositions.RemoveAt(randomIndex);
        
        return randomPosition;
        
    }


    // Calculates the level score in relation to the remaining time. the higher remaining time, the higher the score
    private int CalculateCurrentLevelScore(int reachedScore)
    {

        if(LeftLevelTime >= 1.0f)
        {
            reachedScore = Mathf.FloorToInt((LeftLevelTime * reachedScore) * (LeftLevelTime / _amountOfTargets)) ;
        }

        else
        {
            reachedScore *= 1;
        }
        

        return reachedScore;
    }













}
