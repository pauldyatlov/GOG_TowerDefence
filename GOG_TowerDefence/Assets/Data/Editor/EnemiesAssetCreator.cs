using UnityEngine;
using UnityEditor;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
/// 
public partial class GoogleDataAssetUtility
{
    [MenuItem("Assets/Create/Google/Enemies")]
    public static void CreateEnemiesAssetFile()
    {
        Enemies asset = CustomAssetUtility.CreateAsset<Enemies>();
        asset.SheetName = "gogtowerdefence";
        asset.WorksheetName = "Enemies";
        EditorUtility.SetDirty(asset);        
    }
    
}