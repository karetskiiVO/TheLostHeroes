using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;
public struct PawnDefendSystem : IEcsRunSystem
{

    EcsFilter<Pawn, PawnDefending> pawnFilter;
    public void Run()
    {
        foreach (int i in pawnFilter)
        {
            //TODO:на клиенте визуал (условно, бегать по комнате, играть анимации при файте), на мастере также проверять когда должна закончить защиту
        }
    }
}
