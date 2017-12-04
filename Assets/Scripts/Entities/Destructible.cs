using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public int hp
    {
        get
        {
            return _hp;
        }
    }

    public int hpMax;
    public List<ArkTask> tasksOnFullHP;
    public List<ArkTask> tasksOnNoHP;

    private int _hp;               // Final HP (after buffs)
    private int _hpBase;           // Amount of base HP (pre-buffs)
    
	void Start ()
    {
        _hpBase = hpMax;
        _UpdateHP();
    }

    private void _UpdateHP()
    {
        int tSumHP = _hpBase;
        // TODO: Calculate buffs

        _hp = tSumHP;
    }

    void Damage(int pAmount)
    {
        if (_hpBase <= 0) { return; } // It's already destroyed!

        // Modify base HP
        _hpBase -= pAmount;
        _UpdateHP();

        if (_hp <= 0) // Uh oh! It just broke!
        {
            _hp = 0;
            _hpBase = 0;
            
            foreach(ArkTask tTask in tasksOnNoHP) { tTask.run(); }
        }
    }

    void Restore(int pAmount, bool pRunTasksOnFull = true)
    {
        if(_hpBase >= hpMax) { return; }

        _hpBase += pAmount;
        _UpdateHP();

        if (_hp >= hpMax)
        {
            _hp = hpMax;

            if(pRunTasksOnFull)
            {
                foreach (ArkTask tTask in tasksOnFullHP) { tTask.run(); }
            }
        }
    }

    void Complete(bool pRunTasks = false)
    {
        _hp = hpMax;
        _UpdateHP();

        if(pRunTasks)
        {
            foreach (ArkTask tTask in tasksOnFullHP) { tTask.run(); }
        }
    }
}
