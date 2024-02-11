using System.Collections.Generic;
using System.Linq;
using DataSystem;
using UnityEngine;

public class DataManager : IDataManager
{
    private List<ComposerData> composersDB = new();

    public DataManager()
    {
        LoadDatabase();
    }

    private void LoadDatabase()
    {
        Debug.Log("<color=yellow>Starting Database Load</color>");
        var composersAsset = Resources.LoadAll<ComposerData>("Composers");
        composersDB = composersAsset.ToList();
    }

    public List<ComposerData> GetComposers(int level)
    {
        return composersDB.FindAll(x => x.Level == level);
    }
}