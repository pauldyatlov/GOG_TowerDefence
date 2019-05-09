using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
///
[System.Serializable]
public class WavesData
{
  [SerializeField]
  string key;
  public string KEY { get {return key; } set { key = value;} }
  
  [SerializeField]
  int[] waveformula = new int[0];
  public int[] Waveformula { get {return waveformula; } set { waveformula = value;} }
  
  [SerializeField]
  float timebeforedeploy;
  public float Timebeforedeploy { get {return timebeforedeploy; } set { timebeforedeploy = value;} }
  
}