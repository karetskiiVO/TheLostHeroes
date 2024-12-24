using UnityEngine;
public class RoomPresets : MonoBehaviour {
    [SerializeField]GameObject[] barrackPresets;
    [SerializeField]GameObject[] minePresets;
    [SerializeField]GameObject[] tavernPresets;

    private static RoomPresets instance;

    private void Awake () {
        instance = this;
    }

    public static GameObject GetTavern () {
        return GameObject.Instantiate(instance.tavernPresets[0]);
    }
    public static GameObject GetBarrack () {
        return GameObject.Instantiate(instance.barrackPresets[0]);
    }
    public static GameObject GetMine () {
        return GameObject.Instantiate(instance.minePresets[0]);
    }
}