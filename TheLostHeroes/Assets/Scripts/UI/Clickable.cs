using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class Clickable : MonoBehaviour
{
    private InputController inputController;

    private void Start () {
        inputController = GameObject.Find("Input").GetComponent<InputController>();
    }

    public virtual void Click  () {}
    public virtual void Attack () {
        var task = new Task();
        task.netFields.ownerID = PhotonNetwork.LocalPlayer.ActorNumber;
        task.netFields.reward = 100;
        task.netFields.targetID = gameObject.GetComponent<NetIDHolder>().ID;
        task.netFields.ID = NetEntitySyncroniser.instance.nextID;

        var tag = new TaskAttack();
        NetEntitySyncroniser.instance.EmitCreate(NetEntitySyncroniser.instance.nextID++, new object[] { task, tag });
    }
    public virtual void Defend () {
        var task = new Task();
        task.netFields.ownerID = PhotonNetwork.LocalPlayer.ActorNumber;
        task.netFields.reward = 100;
        task.netFields.targetID = gameObject.GetComponent<NetIDHolder>().ID;
        task.netFields.ID = NetEntitySyncroniser.instance.nextID;

        var tag = new TaskDefend();
        NetEntitySyncroniser.instance.EmitCreate(NetEntitySyncroniser.instance.nextID++, new object[] { task, tag });
    }
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
