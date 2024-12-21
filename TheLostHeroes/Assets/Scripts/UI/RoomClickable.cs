using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;


public class RoomClickable : Clickable
{
    public override void OnClick()
    {
        var task = new Task();
        task.netFields.ownerID = PhotonNetwork.LocalPlayer.ActorNumber;
        task.netFields.reward = 100;
        task.netFields.targetID = gameObject.GetComponent<NetIDHolder>().ID;
        task.netFields.ID = NetEntitySyncroniser.instance.nextID;

        if (Input.GetMouseButton(0))
        {
            switch (inputController.mode)
            {
                case InputController.AttackMode:
                    {
                        var tag = new TaskAttack();
                        NetEntitySyncroniser.instance.EmitCreate(NetEntitySyncroniser.instance.nextID++, new object[] { task, tag });
                    }
                    break;
                case InputController.DefendMode:
                    {
                        var tag = new TaskDefend();
                        NetEntitySyncroniser.instance.EmitCreate(NetEntitySyncroniser.instance.nextID++, new object[] { task, tag });
                    }
                    break;
                default:
                    {
                        var tag = new TaskWork();
                        NetEntitySyncroniser.instance.EmitCreate(NetEntitySyncroniser.instance.nextID++, new object[] { task, tag });
                    }
                    break;
            }
        }
    }
}

