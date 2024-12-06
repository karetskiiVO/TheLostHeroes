using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.AI;

public struct KnightPawn : IEcsIgnoreInFilter { }//Флаг для фильтрации, нет контента

public struct Pawn
{
    public float speed;
    public NavMeshAgent agent;
}
