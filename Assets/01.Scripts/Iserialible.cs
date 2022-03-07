using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public interface ISerializeble
{
    JObject Serialize();
    public void Desirialize(string jsonString);
    string GetJsonKey();
}
