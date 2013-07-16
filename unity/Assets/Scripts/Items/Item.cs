using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Item
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public int ChargesPerUse { get; set; }
    public int Charges { get; set; }

    public Targets UsableOn { get; set; }

    public List<IGameEvent> Events { get; set; }

    public Item(string id, string name, string description, Targets usableOn)
    {
        this.ID = id;
        this.Name = name;
        this.Description = description;

        this.UsableOn = usableOn;

        this.Charges = 1;
        this.ChargesPerUse = 0;

        this.Events = new List<IGameEvent>();
    }

    public virtual void Use()
    {
        this.Charges = this.Charges - this.ChargesPerUse;
    }

    public virtual bool CanUse()
    {
        return !(this.Charges - this.ChargesPerUse < 0);
    }

    public virtual Item Clone()
    {
        return (Item)this.MemberwiseClone();
    }
}
