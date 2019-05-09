using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using GDataDB;
using GDataDB.Linq;

using UnityQuickSheet;

///
/// !!! Machine generated code !!!
///
[CustomEditor(typeof(Enemies))]
public class EnemiesEditor : BaseGoogleEditor<Enemies>
{	    
    public override bool Load()
    {        
        Enemies targetData = target as Enemies;
        
        var client = new DatabaseClient("", "");
        string error = string.Empty;
        var db = client.GetDatabase(targetData.SheetName, ref error);	
        var table = db.GetTable<EnemiesData>(targetData.WorksheetName) ?? db.CreateTable<EnemiesData>(targetData.WorksheetName);
        
        List<EnemiesData> myDataList = new List<EnemiesData>();
        
        var all = table.FindAll();
        foreach(var elem in all)
        {
            EnemiesData data = new EnemiesData();
            
            data = Cloner.DeepCopy<EnemiesData>(elem.Element);
            myDataList.Add(data);
        }
                
        targetData.dataArray = myDataList.ToArray();
        
        EditorUtility.SetDirty(targetData);
        AssetDatabase.SaveAssets();
        
        return true;
    }
}
