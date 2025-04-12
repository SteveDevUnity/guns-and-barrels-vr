using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;


public class DataPersistanceManager : MonoBehaviour
{   
    public static DataPersistanceManager Instance {  get; private set; }

    private GameData gameData;  
    private List<IDataPersistance> dataPersistanceObjects;
    private FileDataHandler fileDataHandler;  

    [SerializeField] private string _fileName;     
    
    public static event Action<bool> OnCheckProfileIsCreated;
    public static event Action OnLoadingCompleted;

    private bool profileIsCreated;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoad;

        if(Instance != null && Instance != this)
        {
            Destroy(Instance);
        }

        else
        {
            Instance = this;                 
            DontDestroyOnLoad(gameObject);
        }      
    }


    private void Start()
    {        
        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileName);
        
        this.dataPersistanceObjects = FindAllDataPersistantObjects();

        LoadGame();
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }


    public void LoadGame()
    {
        this.dataPersistanceObjects = FindAllDataPersistantObjects();
        this.gameData = fileDataHandler.Load();

        if(this.gameData == null)
        {             
            NewGame();
            
            profileIsCreated = false;

        }

        else
        {
            if (!gameData.ProfileIsCreated)
            {
                profileIsCreated = false;
            }

            else
            {
                profileIsCreated = true;
            }
            

            foreach (IDataPersistance dataPersistantObj in dataPersistanceObjects)
            {
                dataPersistantObj.LoadGameData(gameData);
            }

            
        }        

        if(OnCheckProfileIsCreated != null)
        {
            OnCheckProfileIsCreated.Invoke(profileIsCreated);
        }

        OnLoadingCompleted?.Invoke();
        
    }

    
    public void SaveGame()
    {
        foreach(IDataPersistance dataPersistantObj in dataPersistanceObjects)
        {
            dataPersistantObj.SaveGameData(ref gameData);
        }

        fileDataHandler.Save(gameData);
    }

    private List<IDataPersistance> FindAllDataPersistantObjects()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();

        return new List<IDataPersistance>(dataPersistanceObjects);  
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }


    private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        dataPersistanceObjects = FindAllDataPersistantObjects();
    }
    

}



