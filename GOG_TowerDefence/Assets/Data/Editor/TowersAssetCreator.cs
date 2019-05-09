using UnityEngine;
using UnityEditor;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
/// 
public partial class GoogleDataAssetUtility
{
    [MenuItem("Assets/Create/Google/Towers")]
    public static void CreateTowersAssetFile()
    {
        Towers asset = CustomAssetUtility.CreateAsset<Towers>();
        asset.SheetName = "gogtowerdefence";
        asset.WorksheetName = "Towers";
        EditorUtility.SetDirty(asset);        
    }
    
}