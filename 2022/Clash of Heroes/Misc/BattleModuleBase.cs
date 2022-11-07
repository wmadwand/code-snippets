using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleModuleBase
{
    protected BattleState BattleState { get { return BattleSimulator.State; } }
    protected BattleSimulator BattleSimulator;

    public abstract void ApplyTick();

    public virtual void Activate()
    {

    }

    public virtual void Dispose()
    {

    }

    public abstract class CreatorBase
    {
        public BattleModuleBase Create(BattleSimulator battleSimulator, BattleStartConfig startConfig)
        {
            var res = DoCreate(battleSimulator, startConfig);
            res.BattleSimulator = battleSimulator;
            return res;
        }
        protected abstract BattleModuleBase DoCreate(BattleSimulator battleSimulator, BattleStartConfig startConfig);
    }

}


