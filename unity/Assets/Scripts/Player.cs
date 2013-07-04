using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FSprite
{

    private Vector2 tileCoordinates;

    public Vector2 TileCoordinates { get { return new Vector2(TileX, TileY); } set { tileCoordinates = new Vector2(value.x, value.y); } }

    public int TileX { get { return (int) tileCoordinates.x; } set { tileCoordinates.x = value; } }

    public int TileY { get { return (int) tileCoordinates.y; } set { tileCoordinates.y = value; } }

    public Player(string elementName)
        : base(elementName)
    {


    }
   
}
