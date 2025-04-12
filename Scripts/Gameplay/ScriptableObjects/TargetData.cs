using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName ="NewTarget", menuName = "Target")]

public class TargetData : ScriptableObject
{
    public string TargetName;
    public GameObject TargetPrefab;
    public int BaseHealth;
    public int ScorePoints;   
    public int LifeTime;
    public int AmountOfDamage;
    public float BaseMovementSpeed;

    public AudioClip ExplosionSound;
    public bool CanAttack;

}


















