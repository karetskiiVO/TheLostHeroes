using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class InputModeIndicator : MonoBehaviour {
    private static Image prevImage = null;
    private Image currImage = null;
    private readonly Color defaltColor = new Color(244f / 255, 229f / 255, 171f / 255);
    private readonly Color activeColor = new Color(221f / 255, 203f / 255, 129f / 255);

    private void Start () {
        currImage = GetComponent<Image>();
        if (currImage != null) {
            currImage.color = defaltColor;
        }
    }

    public void Handle () {
        if (prevImage != null) {
            prevImage.color = defaltColor;
        }

        if (ReferenceEquals(prevImage, currImage)) {
            prevImage = null;
        } else {
            currImage.color = activeColor;
            prevImage = currImage;
        }
    } 
}
