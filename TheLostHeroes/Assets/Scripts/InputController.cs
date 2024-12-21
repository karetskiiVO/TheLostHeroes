using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    [SerializeField]private ECS ecs;
    [SerializeField]private float cameraSence;

    // положительные значения зафиксируем за заклинаниями
    public const int SelectMode = -1;
    public const int AttackMode = -2;
    public const int DefendMode = -3;
    public const int ScareMode  = -4;
    public const int ClickMode  = -5;
    
    private int _mode = SelectMode;

    public int mode { get => _mode; }

    public void SetMouseMode (int inputMode) {
        if (_mode == inputMode) {
            inputMode = SelectMode;
        }

        _mode = inputMode;
    }

    void Update () {
        var deltaInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (deltaInput.sqrMagnitude > 1) {
            deltaInput = deltaInput.normalized;
        }

        var orthographicSize = Camera.main.orthographicSize;
        var position = Camera.main.transform.position + Time.deltaTime * cameraSence * orthographicSize * deltaInput;

        position.x = Math.Min(position.x, 128);
        position.x = Math.Max(position.x, -128);
        position.y = Math.Min(position.y, 128);
        position.y = Math.Max(position.y, -128);

        Camera.main.transform.position = position;

        var scroll = -Input.GetAxis("Mouse ScrollWheel");
        orthographicSize *= Mathf.Pow(2, scroll);
        orthographicSize = Math.Min(orthographicSize, 40);
        orthographicSize = Math.Max(orthographicSize, 2);
        
        Camera.main.orthographicSize = orthographicSize;
    }
}
