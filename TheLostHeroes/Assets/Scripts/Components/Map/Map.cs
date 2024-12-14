using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public struct Map {
    public Tilemap tilemap;
    public TilemapRenderer renderer;
    public SpriteAtlas sprites;

    public List<EcsEntity> rooms;
    public EcsEntity[,] tiles;
}
