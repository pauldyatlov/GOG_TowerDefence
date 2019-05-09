using UnityEngine;
using UnityEditor;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
/// 
public partial class GoogleDataAssetUtility
{
    [MenuItem("Assets/Create/Google/Waves")]
    public static void CreateWavesAssetFile()
    {
        Waves asset = CustomAssetUtility.CreateAsset<Waves>();
        asset.SheetName = "gogtowerdefence";
        asset.WorksheetName = "Waves";
        EditorUtility.SetDirty(asset);        
    }
    
}