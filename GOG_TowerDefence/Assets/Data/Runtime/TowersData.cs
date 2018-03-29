using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
///
[System.Serializable]
public class TowersData
{
  [SerializeField]
  string key;
  public string KEY { get {return key; } set { key = value;} }
  
  [SerializeField]
  bool main;
  public bool Main { get {return main; } set { main = value;} }
  
  [SerializeField]
  float damage;
  public float Damage { get {return damage; } set { damage = value;} }
  
  [SerializeField]
  string[] upgradetowers = new string[0];
  public string[] Upgradetowers { get {return upgradetowers; } set { upgradetowers = value;} }
  
  [SerializeField]
  float shootdistance;
  public float Shootdistance { get {return shootdistance; } set { shootdistance = value;} }
  
  [SerializeField]
  float firerate;
  public float Firerate { get {return firerate; } set { firerate = value;} }
  
  [SerializeField]
  float turnspeed;
  public float Turnspeed { get {return turnspeed; } set { turnspeed = value;} }
  
  [SerializeField]
  int price;
  public int Price { get {return price; } set { price = value;} }
  
}