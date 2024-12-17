using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriberBehavour : MonoBehaviour {
    [SerializeField]private Text entityName;
    [SerializeField]private Text entityDescription;

    private void Start () {
        SetDefoltDescription();
    }

    public void SetDesctription (string name, string description) {
        entityName.text = name;
        entityDescription.text = description;
    }

    public void SetDefoltDescription () {
        SetDesctription("<EntityName>", "<EntityDescription>");
    }
}
