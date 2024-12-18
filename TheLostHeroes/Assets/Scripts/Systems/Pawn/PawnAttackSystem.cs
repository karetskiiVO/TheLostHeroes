using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;
public struct PawnAttackSystem : IEcsRunSystem
{

    EcsFilter<Pawn, PawnAttacking> pawnFilter;
    public void Run()
    {
        foreach (int i in pawnFilter)
        {
            //TODO:на клиенте визуал, на мастере также проверять когда комната захвачена (выдать награду, передать комнату)
        }
    }
}
