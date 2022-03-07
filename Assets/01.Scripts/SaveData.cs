using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData : ISerializeble
{
    public List<int> rejectItem = new List<int>();

    public void Desirialize(string jsonString)
    {
        JsonUtility.FromJsonOverwrite(jsonString, this);
    }

    public string GetJsonKey()
    {
        return "SaveData";
    }

    public JObject Serialize()
    {
        string jsonString = JsonUtility.ToJson(this);
        Debug.Log(jsonString);
        JObject returnVal = JObject.Parse(jsonString);

        return returnVal;
    }
}