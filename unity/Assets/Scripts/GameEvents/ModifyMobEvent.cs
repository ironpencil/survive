using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ModifyMobEvent : IGameEvent
{
    public Mob Mob { get; set; }

    private MobStats stat;
    private float value;

    public ModifyMobEvent(Mob mob, MobStats stat, float value)
    {
        this.Mob = mob;
        this.stat = stat;
        this.value = value;
    }

    public ModifyMobEvent(MobStats stat, float value)
    {
        this.Mob = null;
        this.stat = stat;
        this.value = value;
    }

    public void Execute()
    {
        Mob.TryModifyStat(stat, value);
    }
}
