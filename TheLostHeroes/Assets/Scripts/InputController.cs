using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    [SerializeField] private float cameraSence;

    void Update () {
        Camera.main.transform.position += cameraSence * Time.deltaTime * new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
    }
}
