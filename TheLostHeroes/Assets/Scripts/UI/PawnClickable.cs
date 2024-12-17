using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnClickable : Clickable
{
    public override void OnClick()
    {
        Debug.Log(gameObject.name);
    }
}
