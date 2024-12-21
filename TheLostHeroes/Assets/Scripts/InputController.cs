using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    [SerializeField]private ECS ecs;
    [SerializeField]private float cameraSence;

    // положительные значения зафиксируем за заклинаниями
    const int SelectMode = -1;
    const int AttackMode = -2;
    const int DefendMode = -3;
    const int ScareMode  = -4;
    const int ClickMode  = -5;
    
    private int mode = SelectMode;
    public void SetMouseMode (int inputMode) {
        if (mode == inputMode) {
            inputMode = SelectMode;
        }

        mode = inputMode;
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

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            switch (mode) {
            case SelectMode: 
                ecs.Select();
                break;
            case AttackMode:
                ecs.Attack();
                break;
            case DefendMode:
                ecs.Defend();
                break;
            case ScareMode:
                ecs.Scare();
                break;
            case ClickMode:
                ecs.Click();
                break;
            default:
                ecs.CastSpell(mode);
                break;
            }
        }
    }
}
