using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;


public class RoomClickable : Clickable
{
    public override void Attack () {
        var task = new Task();
        task.netFields.ownerID = PhotonNetwork.LocalPlayer.ActorNumber;
        task.netFields.reward = 100;
        task.netFields.targetID = gameObject.GetComponent<NetIDHolder>().ID;
        task.netFields.ID = NetEntitySyncronizer.instance.nextID;
        task.netFields.x =  Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        task.netFields.y =  Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

        var tag = new TaskAttack();
        NetEntitySyncronizer.instance.EmitCreate(NetEntitySyncronizer.instance.nextID++, new object[] { task, tag });
    }
    public override void Defend () {
        var task = new Task();
        task.netFields.ownerID = PhotonNetwork.LocalPlayer.ActorNumber;
        task.netFields.reward = 100;
        task.netFields.targetID = gameObject.GetComponent<NetIDHolder>().ID;
        task.netFields.ID = NetEntitySyncronizer.instance.nextID;
        task.netFields.x =  Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        task.netFields.y =  Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

        var tag = new TaskDefend();
        NetEntitySyncronizer.instance.EmitCreate(NetEntitySyncronizer.instance.nextID++, new object[] { task, tag });
    }
}

