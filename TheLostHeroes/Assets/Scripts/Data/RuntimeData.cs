using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using System;
using System.Text;
using UnityEngine;
using NavMeshPlus.Components;

public struct RuntimeData
{
    //stuff only created in runtime
    public EcsEntity map;

    public List<EcsEntity> rooms;
    public RandomConfiguration randomConfiguration;
    public EmptyClickable defaultClickableBehavour;

    public int playerMoney;
}
