using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class MoneyTileController : MonoBehaviour {
    float endpoint;
    [SerializeField]float speed = 10;
    private Text msg = null;
    private Vector3 position;

    public void ShowMoney (float duration, Vector2 position, int val) {
        endpoint = duration;
        this.position = position;

        gameObject.SetActive(true);
        transform.position = Camera.main.WorldToScreenPoint(position);

        GetComponent<Text>().text = "+" + val.ToString() + "g";
    }

    private void Update() {
        if (endpoint < 0) {
            gameObject.SetActive(false);
            return;
        }

        position += speed * Time.deltaTime * new Vector3(0, 1);
        endpoint -= Time.deltaTime;
        
        transform.position = Camera.main.WorldToScreenPoint(position);
    }
}
