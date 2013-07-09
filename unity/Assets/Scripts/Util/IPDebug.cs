using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class IPDebug
{

    public static bool DoLog { get; set; }

    public static void Log(string text)
    {
        if (DoLog)
        {
            Debug.Log(text);
        }
    }
}
