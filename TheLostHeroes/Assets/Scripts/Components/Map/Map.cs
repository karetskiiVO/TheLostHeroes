using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct Map {
    public Tilemap tilemap;
    public TilemapRenderer renderer;
    public Sprite[] sprites;

    public List<EcsEntity> rooms;
    public EcsEntity[,] tiles;
}
