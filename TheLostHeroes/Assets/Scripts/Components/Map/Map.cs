using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;

public struct Map
{
    public List<EcsEntity> rooms;
    public EcsEntity[,] tiles;
}
