using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct PawnIdle : IEcsIgnoreInFilter { }
[System.Serializable]
public struct PawnGoing : IEcsIgnoreInFilter { }
[System.Serializable]
public struct PawnDefending : IEcsIgnoreInFilter { }
[System.Serializable]
public struct PawnWorking : IEcsIgnoreInFilter { }
[System.Serializable]
public struct PawnAttacking : IEcsIgnoreInFilter { }

public struct Pawn
{
    [System.Serializable]
    public struct Networked
    {
        public float speed;
        public float x;
        public float y;
        public int taskID;
        public int ownerID;
        public int ID;
    }

    public Networked netFields;
    public GameObject self;
}
