using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Leopotam.Ecs;
public class TaskClickable : Clickable
{
    public override void OnClick()
    {
        int id = gameObject.GetComponent<NetIDHolder>().ID;
        if (Input.GetMouseButtonDown(0))//increase reward by 100
        {
            ref var taskComponent = ref NetEntitySyncroniser.MustGetComponent<Task>(id);
            taskComponent.netFields.reward += 100;
            NetEntitySyncroniser.instance.EmitUpdate(id, new object[] { taskComponent });
            Debug.Log("New reward: " + taskComponent.netFields.reward);
        }
        else if (Input.GetMouseButtonDown(1))//remove task
        {
            NetEntitySyncroniser.instance.EmitDestroy(id);
        }
    }
}
