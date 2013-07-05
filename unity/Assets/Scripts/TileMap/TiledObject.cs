using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TiledObject
{


    public ObjectLayer Layer { get; set; }

    public string Name { get; set; }

    public string ObjType { get; set; }

    public bool Visible { get; set; }

    public int PosX { get; set; }

    public int PosY { get; set; }

    public int ObjWidth { get; set; }

    public int ObjHeight { get; set; }

    public Dictionary<string, object> ObjProperties { get; set; }

    public string GetPropertyValue(string propertyName)
    {
        string propertyValue = "";

        if (ObjProperties != null)
        {

            if (ObjProperties.ContainsKey(propertyName))
            {

                object objValue;
                if (ObjProperties.TryGetValue(propertyName, out objValue))
                {
                    propertyValue = objValue.ToString();
                }

            }
        }

        return propertyValue;
    }

    public List<string> GetPropertyNames()
    {
        List<string> propertyNames = new List<string>();

        foreach (string key in ObjProperties.Keys)
        {
            propertyNames.Add(key);
        }

        return propertyNames;
    }
}
