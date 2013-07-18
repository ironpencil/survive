using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Mob : FSprite
{

    protected Dictionary<MobStats, float> mobStats = new Dictionary<MobStats, float>();

    protected Vector2 tileCoordinates;

    public Vector2 TileCoordinates { get { return new Vector2(TileX, TileY); } set { tileCoordinates = new Vector2(value.x, value.y); } }

    public int TileX { get { return (int) tileCoordinates.x; } set { tileCoordinates.x = value; } }

    public int TileY { get { return (int) tileCoordinates.y; } set { tileCoordinates.y = value; } }

    public float MoveDelayTime = 0.1f;
    public float NextMoveTime = 0.0f;

    public bool IsMovingToPosition = false;
    public Vector2 TargetPosition = Vector2.zero;
    protected Vector2 speed = Vector2.zero;

    public List<Item> Inventory { get; set; }

    private int energy;
    public int Energy
    {
        get { return energy; }
        set
        {
            if (value < 0) { value = 0; }
            if (value > GameVars.Instance.PLAYER_FULL_ENERGY) { value = GameVars.Instance.PLAYER_FULL_ENERGY; }
            energy = value;
        }
    }

    private int water;
    public int Water
    {
        get { return water; }
        set
        {
            if (value < 0) { value = 0; }
            if (value > GameVars.Instance.PLAYER_FULL_WATER) { value = GameVars.Instance.PLAYER_FULL_WATER; }
            water = value;
        }
    }

    public Mob(string elementName)
        : base(elementName)
    {
        speed.x = this.width / MoveDelayTime;
        speed.y = this.height / MoveDelayTime;
        Inventory = new List<Item>();
        Energy = GameVars.Instance.PLAYER_FULL_ENERGY;
        Water = GameVars.Instance.PLAYER_FULL_WATER;
    }





    public bool ApproachTarget()
    {
        //return true if we're already at our target
        if (this.TargetPosition == this.GetPosition()) { return true; }

        Vector2 dtSpeed = new Vector2(speed.x * Time.deltaTime, speed.y * Time.deltaTime);

        Vector2 difference = this.TargetPosition - this.GetPosition();

        if (difference.x > dtSpeed.x)
        {
            this.x += dtSpeed.x;
        }
        else if (difference.x < -dtSpeed.x)
        {
            this.x -= dtSpeed.x;
        }
        else
        {
            this.x = this.TargetPosition.x;
        }

        if (difference.y > dtSpeed.y)
        {
            this.y += dtSpeed.y;
        }
        else if (difference.y < -dtSpeed.y)
        {
            this.y -= dtSpeed.y;
        }
        else
        {
            this.y = this.TargetPosition.y;
        }

        return (this.TargetPosition == this.GetPosition());

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

    public float GetStat(MobStats stat)
    {
        float statValue = 0.0f;
        mobStats.TryGetValue(stat, out statValue);
        return statValue;
    }

    public void SetStat(MobStats stat, float value)
    {
        if (mobStats.ContainsKey(stat))
        {
            mobStats[stat] = value;
        }
        else
        {
            mobStats.Add(stat, value);
        }
    }

    //returns true if the stat exists and was modified
    public bool TryModifyStat(MobStats stat, float value)
    {
        float statValue = 0.0f;
        bool statExists = mobStats.TryGetValue(stat, out statValue);

        if (statExists)
        {
            mobStats[stat] = statValue + value;
        }
        
        return statExists;
    }
   
}
