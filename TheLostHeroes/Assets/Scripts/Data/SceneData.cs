using UnityEngine;

public class SceneData : MonoBehaviour {
    public string seed;

    [System.Serializable]
    public class GameResources {
    }
    public GameResources gameResources;

    public DescriberBehavour sceneDescriber;
}
