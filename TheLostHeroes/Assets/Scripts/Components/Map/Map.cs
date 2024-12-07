using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;

public struct Map {
    public SpriteRenderer renderer;

    public List<EcsEntity> rooms;
    public EcsEntity[,] tiles;
}
