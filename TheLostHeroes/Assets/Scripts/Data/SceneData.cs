using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneData : MonoBehaviour {
    public string seed;

    [System.Serializable]
    public class GameResources {
    }
    public GameResources gameResources;
}
