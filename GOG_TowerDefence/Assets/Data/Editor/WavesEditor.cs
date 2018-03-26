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
[CustomEditor(typeof(Waves))]
public class WavesEditor : BaseGoogleEditor<Waves>
{	    
    public override bool Load()
    {        
        Waves targetData = target as Waves;
        
        var client = new DatabaseClient("", "");
        string error = string.Empty;
        var db = client.GetDatabase(targetData.SheetName, ref error);	
        var table = db.GetTable<WavesData>(targetData.WorksheetName) ?? db.CreateTable<WavesData>(targetData.WorksheetName);
        
        List<WavesData> myDataList = new List<WavesData>();
        
        var all = table.FindAll();
        foreach(var elem in all)
        {
            WavesData data = new WavesData();
            
            data = Cloner.DeepCopy<WavesData>(elem.Element);
            myDataList.Add(data);
        }
                
        targetData.dataArray = myDataList.ToArray();
        
        EditorUtility.SetDirty(targetData);
        AssetDatabase.SaveAssets();
        
        return true;
    }
}
