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
        public int id;
        public string objective;
        public float targetX;
        public float targetY;
    }
    public Networked netFields;


    public GameObject self;
}
