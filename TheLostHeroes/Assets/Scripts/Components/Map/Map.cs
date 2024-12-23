using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public struct Map {
    public Tilemap walkable_tilemap;
    public Tilemap obstacle_tilemap_front;
    public Tilemap obstacle_tilemap_back;
    public SpriteAtlas sprites;

    public List<EcsEntity> rooms;
    public EcsEntity[,] tiles;
}
