using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;


public class RoomClickable : Clickable
{
    public override void OnClick()
    {
        Task task = new Task();
        task.netFields.ownerID = PhotonNetwork.LocalPlayer.ActorNumber;
        task.netFields.reward = 100;
        task.netFields.targetID = gameObject.GetComponent<NetIDHolder>().ID;
        task.netFields.ID = NetEntitySyncroniser.instance.nextID;
        if (Input.GetMouseButton(0))
        {
            TaskAttack tag = new TaskAttack();
            NetEntitySyncroniser.instance.EmitCreate(NetEntitySyncroniser.instance.nextID++, new object[] { task, tag });
        }
        else if (Input.GetMouseButton(1))
        {
            TaskDefend tag = new TaskDefend();
            NetEntitySyncroniser.instance.EmitCreate(NetEntitySyncroniser.instance.nextID++, new object[] { task, tag });
        }
        else
        {
            TaskWork tag = new TaskWork();
            NetEntitySyncroniser.instance.EmitCreate(NetEntitySyncroniser.instance.nextID++, new object[] { task, tag });
        }
    }
}

