using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FSprite
{

    private Vector2 tileCoordinates;

    public Vector2 TileCoordinates { get { return new Vector2(TileX, TileY); } set { tileCoordinates = new Vector2(value.x, value.y); } }

    public float TileX { get { return tileCoordinates.x; } set { tileCoordinates.x = value; } }

    public float TileY { get { return tileCoordinates.y; } set { tileCoordinates.y = value; } }

    public Player(string elementName)
        : base(elementName)
    {


    }
   
}
