using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Clickable : MonoBehaviour
{
    protected InputController inputController;

    private void Start () {
        inputController = GameObject.Find("Input").GetComponent<InputController>();
    }

    public abstract void OnClick();
}
