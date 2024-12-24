using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class StaticData : ScriptableObject
{
    //config
    public GameObject pawnPrefab;
    public GameObject roomPrefab;
    public GameObject taskPrefab;
    [System.Serializable]
    public class TileData
    {
        public bool impassable;
    }

    public List<TileData> TileTypes;

    //network
    public const string PLAYER_READY = "IsPlayerReady";
    public const string PLAYER_LOADED_MAP = "PlayerLoadedMap";
    //player colors
    public static Color GetColor(int colorChoice)
    {
        switch (colorChoice)
        {
            case -1: return Color.white;
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
            case 4: return Color.cyan;
            case 5: return Color.grey;
            case 6: return Color.magenta;
            case 7: return Color.white;
        }

        return Color.black;
    }
}
