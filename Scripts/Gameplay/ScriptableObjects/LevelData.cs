using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewLevelData", menuName = "LevelData")]

public class LevelData : ScriptableObject
{    
    public float SpeedMultiplier;
    public float HealthMultiplier;
    public int TotalLevelTime;

    public int AmountOfTargets;

    public List<TargetData> TargetDataList;

    public MovementType MovementType;

    public MovementStrategyScriptableObject MovementStrategy;

    public string GetMovementType()
    {
        return MovementType.ToString();
    }
}

public enum MovementType
{
    Horizontal,
    Vertical
}