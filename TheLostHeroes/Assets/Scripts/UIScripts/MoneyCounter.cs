using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCounter : MonoBehaviour {
    private int playerMoney = 100;
    bool updated = true;
    private Text text = null;
    static MoneyCounter instance = null;

    static Queue<MoneyTileController> moneyCounters = new Queue<MoneyTileController>();

    [SerializeField]GameObject moneyTileExemplar;

    private MoneyTileController NewMoneyTile () {
        var newMoneyTile = GameObject.Instantiate(moneyTileExemplar, transform.parent.transform);
        var moneyTileController = newMoneyTile.GetComponent<MoneyTileController>();
        newMoneyTile.SetActive(false);

        return moneyTileController;
    }

    private void Start () {
        text = GetComponent<Text>();
        instance = this;

        moneyCounters.Enqueue(NewMoneyTile());
    }

    public static bool StaticDecreaseIfHas (int val) {
        if (instance == null) {
            return false;
        }

        return instance.DecreaseIfHas(val);
    }

    public bool DecreaseIfHas (int val) {
        if (playerMoney >= val) {
            playerMoney -= val;
            updated = true;
            return true;
        }

        return false;
    }

    public static void AddMoney (int val, Vector2 position) {
        if (instance == null) return;

        MoneyTileController moneyTile = null;

        if (moneyCounters.Peek().gameObject.activeSelf) {
            moneyTile = moneyCounters.Dequeue();
        } else {
            moneyTile = instance.NewMoneyTile();
        }
        moneyTile.ShowMoney(Random.Range(0.5f, 1f), position, val);
        moneyCounters.Enqueue(moneyTile);


        instance.playerMoney += val;
        instance.updated = true;
    }

    private void Update () {
        if (!updated) return;

        if (playerMoney < 800) {
            text.text = " Coins: " + playerMoney.ToString();
        } else {
            text.text = " Coins: " + (playerMoney / 1000).ToString() + "k" + (playerMoney % 1000 / 100).ToString();
        }
    }
}
