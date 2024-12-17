using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.AI;
[System.Serializable]
public struct KnightPawn : IEcsIgnoreInFilter { }//Флаг для фильтрации, нет контента

public struct Pawn
{
    [System.Serializable]
    public struct Networked
    {
        public float speed;
        public float atk;
        public float x;
        public float y;
        public int taskID;
        public int ownerID;
        public int ID;
    }
    public Networked netFields;

    public GameObject self;
}
