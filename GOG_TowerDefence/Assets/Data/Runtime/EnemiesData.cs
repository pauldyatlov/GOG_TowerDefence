using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
///
[System.Serializable]
public class EnemiesData
{
  [SerializeField]
  string key;
  public string KEY { get {return key; } set { key = value;} }
  
  [SerializeField]
  int maxhealth;
  public int Maxhealth { get {return maxhealth; } set { maxhealth = value;} }
  
  [SerializeField]
  float movespeed;
  public float Movespeed { get {return movespeed; } set { movespeed = value;} }
  
  [SerializeField]
  int reward;
  public int Reward { get {return reward; } set { reward = value;} }
  
}