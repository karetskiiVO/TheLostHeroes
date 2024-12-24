using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour {
    [SerializeField]GameObject winMsg;
    [SerializeField]GameObject loseMsg;

    static VictoryManager instance;

    private void Awake () {
        instance = this;
    }

    public static void Win () {
        instance.winMsg.SetActive(true);
    }

    public static void Lose () {
        instance.loseMsg.SetActive(true);
    }
}
