using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MapLayer : FContainer
{
    public string Name { get; set; }

    public bool Visible { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public string LayerType { get; set; }

    public int Opacity { get; set; }
}
