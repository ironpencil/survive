using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Mob : FAnimatedSprite
{

    protected Dictionary<MobStats, float> mobStats = new Dictionary<MobStats, float>();

    protected Vector2 tileCoordinates;

    public Vector2 TileCoordinates { get { return new Vector2(TileX, TileY); } set { tileCoordinates = new Vector2(value.x, value.y); } }

    public int TileX { get { return (int) tileCoordinates.x; } set { tileCoordinates.x = value; } }

    public int TileY { get { return (int) tileCoordinates.y; } set { tileCoordinates.y = value; } }

    public float MoveDelayTime = 0.2f;

    public bool IsMovingToPosition = false;
    public Vector2 TargetPosition = Vector2.zero;
    public Vector2 speed = Vector2.zero;

    private int level = 0;
    public int Level
    {
        get
        {
            if (level < 0) { level = 0; }

            return level;
        }
        set
        {
            level = value;

            int remainder = 0;

            if (this.WildernessPoints > 0)
            {
                remainder = this.WildernessPoints % 100;
            }

            this.wildernessPoints = (level * 100) + remainder;

            this.MaxEnergy = GameVars.Instance.PLAYER_STARTING_ENERGY + (level * 10);
        }
    }

    public int AttackPower { get; set; }
    public int AttackMultiplier
    {
        get
        {
            return (int) ((Level / 2) + 2);
        }
    }

    public int HitChance
    {
        get
        {
            //int hitChance = 50 + (WildernessPoints / 10);

            int hitChance = 100;
            return hitChance;            
        }
    }

    public int CritChance
    {
        get
        {
            int critChance = Level;

            if (critChance < 5) { critChance = 5; }

            return critChance;
        }
    }

    public int EvadeChance
    {
        get
        {
            //int hitChance = 50 + (WildernessPoints / 10);

            int evadeChance = Level * 2;
            return evadeChance;
        }
    }

    public int Defense { get; set; }

    public bool HasAntArmor { get; set; }

    public bool IsFloating { get; set; }

    public List<Item> Inventory { get; set; }

    FAnimation standingAnim = new FAnimation("standing", new int[1] { 0 }, 500, false);
    FAnimation walkingAnim = new FAnimation("walking", new int[2] { 1, 2 }, 200, true);

    public int MaxEnergy { get; set; }

    private int energy;
    public int Energy
    {
        get { return energy; }
        set
        {
            if (value < 0) { value = 0; }
            if (value > MaxEnergy) { value = MaxEnergy; }
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

    private int wildernessPoints = 0;
    public int WildernessPoints
    {
        get { return wildernessPoints; }
        set
        {
            wildernessPoints = value;
            this.Level = (int)(wildernessPoints / 100);
        }
    }

    public Mob(string elementName)
        : base(elementName)
    {
        this.anchorY = 0.25f;
        speed.x = this.width / MoveDelayTime;
        speed.y = this.height / MoveDelayTime;
        Inventory = new List<Item>();
        MaxEnergy = GameVars.Instance.PLAYER_STARTING_ENERGY;
        Energy = GameVars.Instance.PLAYER_STARTING_ENERGY;
        Water = GameVars.Instance.PLAYER_STARTING_WATER;
        AttackPower = 1;
        Defense = 1;
        HasAntArmor = false;
        IsFloating = false;

        this.addAnimation(standingAnim);
        this.addAnimation(walkingAnim);               
    }

    public void FullHeal()
    {
        this.Energy = this.MaxEnergy;
        this.Water = GameVars.Instance.PLAYER_FULL_WATER;
    }


    public bool ApproachTarget()
    {
        //return true if we're already at our target
        if (this.TargetPosition == this.GetPosition())
        {
            return true;
        }

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

    public Rect GetStandingRect()
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

        //this shifts the rect down half height so you get a rect centered on the "base" of the sprite
        bottom -= (heightMag * 0.25f);

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
