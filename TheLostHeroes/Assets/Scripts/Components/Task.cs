using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;

public struct TaskAttack : IEcsIgnoreInFilter { }
public struct TaskDefend : IEcsIgnoreInFilter { }
public struct TaskExplore : IEcsIgnoreInFilter { }
public struct TaskInteract : IEcsIgnoreInFilter { }

public struct Task
{
    [System.Serializable]
    public struct Networked
    {
        public int targetID;
        public int reward;
        public int ownerID;
        public int ID;
    }
    public Networked netFields;
    public NetIDHolder instance;
}
