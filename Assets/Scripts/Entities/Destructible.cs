using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public int hp { get; set; }
    public int hpMax;
    public List<ArkTask> tasksOnDestruction;
    
	void Start ()
    {
        hp = hpMax;
    }

    void Damage(int pAmount)
    {
        if (hp <= 0) { return; }

        hp -= pAmount;
        if(hp <= 0)
        {
            hp = 0;

            foreach(ArkTask tTask in tasksOnDestruction) { tTask.run(); }
        }
    }
}
