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
[CustomEditor(typeof(Towers))]
public class TowersEditor : BaseGoogleEditor<Towers>
{	    
    public override bool Load()
    {        
        Towers targetData = target as Towers;
        
        var client = new DatabaseClient("", "");
        string error = string.Empty;
        var db = client.GetDatabase(targetData.SheetName, ref error);	
        var table = db.GetTable<TowersData>(targetData.WorksheetName) ?? db.CreateTable<TowersData>(targetData.WorksheetName);
        
        List<TowersData> myDataList = new List<TowersData>();
        
        var all = table.FindAll();
        foreach(var elem in all)
        {
            TowersData data = new TowersData();
            
            data = Cloner.DeepCopy<TowersData>(elem.Element);
            myDataList.Add(data);
        }
                
        targetData.dataArray = myDataList.ToArray();
        
        EditorUtility.SetDirty(targetData);
        AssetDatabase.SaveAssets();
        
        return true;
    }
}
