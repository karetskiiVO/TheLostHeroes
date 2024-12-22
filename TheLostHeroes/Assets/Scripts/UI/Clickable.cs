using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using Photon.Pun;
using UnityEngine;

public abstract class Clickable : MonoBehaviour
{
    private static InputController inputController = null;

    protected static DescriberBehavour describer = null; 
    protected static NetEntitySyncronizer netEntitySynchronizer = null;
    protected static int selectedID = 0;

    protected int ecsid;

    private void Start () {
        var netIDholder = gameObject.GetComponent<NetIDHolder>();
        if (netIDholder != null) {
            ecsid = netIDholder.ID;
        }

        inputController ??= GameObject.Find("Input").GetComponent<InputController>();
        netEntitySynchronizer ??= GameObject.Find("Network").GetComponent<NetEntitySyncronizer>();
        describer ??= GameObject.Find("Describer").GetComponent<DescriberBehavour>();
    }

    protected Clickable previousSelected;

    public virtual void Click  () {}
    public virtual void Attack () {}
    public virtual void Defend () {}
    public virtual void Scare  () {}
    protected void deselect () {
        if (netEntitySynchronizer.entities.ContainsKey(selectedID)) {
            netEntitySynchronizer.entities[selectedID].Del<DescriptionBeholder>();
        }
        selectedID = ecsid;
    }
    public virtual void Select () {
        deselect();

        ref var setter = ref netEntitySynchronizer.entities[ecsid].Get<DescriptionBeholder>();
        setter.describer = describer;
    }
    public virtual void CastSpell (int spellid) {}

    public void OnClick () {
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
