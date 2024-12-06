using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StaticData : ScriptableObject
{
    //config
    public GameObject pawnPrefab;
    [System.Serializable]
    public class TileData
    {
        bool impassable;
    }
    public List<TileData> TileTypes;
}
