using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StaticData : ScriptableObject
{
    //config
    public GameObject pawnPrefab;
    [System.Serializable]
    public class TileData {
        public bool impassable;
    }

    public List<TileData> TileTypes;

    [System.Serializable]
    public class GameResources {
        public Texture2D dungeonTiles;
    }
    public GameResources gameResources;
}
