using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Clickable : MonoBehaviour
{
    protected InputController inputController;

    private void Start () {
        inputController = GameObject.Find("Input").GetComponent<InputController>();
    }

    public virtual void Click  () {}
    public virtual void Attack () {}
    public virtual void Defend () {}
    public virtual void Scare  () {}
    public virtual void Select () {}
    public virtual void CastSpell (int spellid) {}

    public abstract void OnClick (); // TODO: Перейти с этого архаичного ужаса на switch и перегрузки уже нормальных взаимодействий
}
