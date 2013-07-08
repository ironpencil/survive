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

    public Rect GetRect()
    {
        //get the magnitutde of the width and height
        float widthMag = Mathf.Abs(this.width);
        float heightMag = Mathf.Abs(this.height);

        //this finds the left side of the sprite so long as the width is positive
        //finds the right side of the sprite if width is negative
        float left = this.x - (this.width * (this.anchorX));

        //if the width is negative, subtract the magnitutde of the width to find the left side
        if (this.width < 0)
        {
            left -= widthMag;
        }

        float bottom = this.y - (this.height * (this.anchorY));

        if (this.height < 0)
        {
            bottom -= heightMag;
        }

        //return a rect with the calculated top, left, and sizes
        return new Rect(left, bottom, widthMag, heightMag);
    }
   
}
