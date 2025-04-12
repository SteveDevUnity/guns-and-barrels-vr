using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


[System.Serializable]   
public class LevelHighScore {

    public int Level;
    public int Score;
    public bool LevelCompleted;

    public LevelHighScore(int level, int score, bool levelCompleted)
    {
        this.Level = level;
        this.Score = score;
        this.LevelCompleted = levelCompleted;
    }

}


[System.Serializable]
public class GameData
{
    public Vector3 PlayerPosition;
    public float[] PlayerRotation;

    public Vector3 WeaponPosition;
    public float[] WeaponRotation;

    public bool TutorialIsCompleted;

    public string PlayerName;
    public int LevelOfProfile;
    public int TotalPointsCollected;

    public string PlayerProfileSprite;
    
    public int LocalIndex;
    public bool ProfileIsCreated;

    

    public List<LevelHighScore> LevelHighScoreList;

    public int NumberOfLevels;

    public GameData() {

        NumberOfLevels = 10;
        ProfileIsCreated = false;
        TutorialIsCompleted = false;
        LevelHighScoreList = new List<LevelHighScore>();

        for (int i = 1; i < NumberOfLevels + 1 ; i++)
        {
            LevelHighScoreList.Add(new LevelHighScore(i, 0, false));
        }

        TotalPointsCollected = 0;
        LevelOfProfile = 0;
    }
}
