using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
}
