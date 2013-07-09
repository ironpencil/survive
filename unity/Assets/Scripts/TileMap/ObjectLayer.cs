using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ObjectLayer : MapLayer
{

    private List<TiledObject> objects = new List<TiledObject>();
    public List<TiledObject> Objects { get { return objects; } }

    public void AddObject(TiledObject tiledObject)
    {
        objects.Add(tiledObject);

        this.AddChild(tiledObject);
    }

    public void RemoveObject(TiledObject tiledObject)
    {
        objects.Remove(tiledObject);

        this.RemoveChild(tiledObject);
    }

    public List<TiledObject> GetTiledObjectsAt(float x, float y)
    {
        List<TiledObject> objectList = new List<TiledObject>();

        foreach (TiledObject tiledObject in objects)
        {
            if (tiledObject.x == x && tiledObject.y == y)
            {
                objectList.Add(tiledObject);
            }
        }

        return objectList;
    }

    public List<TiledObject> GetTiledObjectsContainingPoint(float x, float y)
    {
        List<TiledObject> objectList = new List<TiledObject>();

        foreach (TiledObject tiledObject in objects)
        {
            if (tiledObject.GetRect().Contains(new Vector2(x, y)))
            {
                objectList.Add(tiledObject);
            }
        }

        return objectList;
    }

    public List<TiledObject> GetTiledObjectsIntersectingRect(Rect checkRect)
    {
        List<TiledObject> objectList = new List<TiledObject>();

        IPDebug.Log("Check Rect = " + GetRectDescription(checkRect));

        foreach (TiledObject tiledObject in objects)
        {
            IPDebug.Log("Object Rect[" + tiledObject.Name + "] = " + GetRectDescription(tiledObject.GetRect()));
            if (tiledObject.GetRect().CheckIntersect(checkRect))
            {
                objectList.Add(tiledObject);
            }
        }

        IPDebug.Log("Objects Returned: " + objectList.Count());
        return objectList;
    }

    public string GetRectDescription(Rect rect)
    {
        string desc = "{";
        desc += "xMin:" + rect.xMin + " ";
        desc += "xMax:" + rect.xMax + " ";
        desc += "yMin:" + rect.yMin + " ";
        desc += "yMax:" + rect.yMax + "}";

        return desc;
    }
    
}
