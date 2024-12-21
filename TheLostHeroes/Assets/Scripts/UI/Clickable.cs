using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class Clickable : MonoBehaviour
{
    private InputController inputController;
    static protected DescriberBehavour describer = null; 

    private void Start () {
        inputController = GameObject.Find("Input").GetComponent<InputController>();
        describer ??= GameObject.Find("Describer").GetComponent<DescriberBehavour>();
    }

    public virtual void Click  () {}
    public virtual void Attack () {}
    public virtual void Defend () {}
    public virtual void Scare  () {}
    public virtual void Select () {}
    public virtual void CastSpell (int spellid) {}

    public void OnClick () { // TODO: Перейти с этого архаичного ужаса на switch и перегрузки уже нормальных взаимодействий
        var mode = inputController.mode;

        switch (mode) {
        case InputController.AttackMode:
            Attack();
            break;
        case InputController.DefendMode:
            Defend();
            break;
        case InputController.ScareMode:
            Scare();
            break;
        case InputController.ClickMode:
            Click();
            break;
        case InputController.SelectMode:
            Select();
            break;
        default:
            CastSpell(mode);
            break;
        }
    }
} 
