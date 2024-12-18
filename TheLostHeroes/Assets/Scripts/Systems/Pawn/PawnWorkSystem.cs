using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;
public struct PawnWorkSystem : IEcsRunSystem
{

    EcsFilter<Pawn, PawnWorking> pawnFilter;
    public void Run()
    {
        foreach (int i in pawnFilter)
        {
            //TODO: на клиенте визуал (условно, бегать по комнате), на мастере также проверять когда должна закончить работу
        }
    }
}
