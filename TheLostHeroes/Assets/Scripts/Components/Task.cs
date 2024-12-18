using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;
[System.Serializable]
public struct TaskAttack : IEcsIgnoreInFilter { }
[System.Serializable]
public struct TaskDefend : IEcsIgnoreInFilter { }
[System.Serializable]
public struct TaskWork : IEcsIgnoreInFilter { }

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
