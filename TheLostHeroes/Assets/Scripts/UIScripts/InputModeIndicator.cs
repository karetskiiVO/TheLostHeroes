using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class InputModeIndicator : MonoBehaviour {
    private static Image prevImage = null;
    private Image currImage = null;

    private void Start () {
        currImage = GetComponent<Image>();
    }

    public void Handle () {
        if (prevImage != null) {
            prevImage.color = Color.white;
        }

        if (ReferenceEquals(prevImage, currImage)) {
            prevImage = null;
        } else {
            currImage.color = new Color(244f / 255, 229f / 255, 171f / 255);
            prevImage = currImage;
        }
    } 
}
