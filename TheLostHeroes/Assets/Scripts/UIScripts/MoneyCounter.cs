using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCounter : MonoBehaviour {
    private PlayerMoney playerMoney = null;
    int prevValue = 0;
    private Text text = null;

    void Start () {
        text = GetComponent<Text>();
    }

    private void Update () {
        playerMoney ??= GameObject.Find("ECS").GetComponent<ECS>().runtimeData.playerMoney;

        if (playerMoney.money != prevValue) {
            prevValue = playerMoney.money;

            if (prevValue < 800) {
                text.text = " Coins: " + prevValue.ToString();
            } else {
                text.text = " Coins: " + (prevValue / 1000).ToString() + "k" + (prevValue % 1000 / 100).ToString();
            }
        }
    }
}
