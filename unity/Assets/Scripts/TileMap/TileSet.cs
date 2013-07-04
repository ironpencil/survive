using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TileSet
{


    public int FirstGID { get; set; }

    public string Image { get; set; }

    public string Name { get; set; }

    public Dictionary<string, object> TileProperties { get; set; }
}
