using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;
public struct DamageApplicator : IEcsRunSystem
{
    EcsFilter<Damage> damagedFilter;

    public void Run()
    {
        foreach (int i in damagedFilter)
        {
            var damage = damagedFilter.Get1(i);
            var entity = damagedFilter.GetEntity(i);
            ref var health = ref entity.Get<Health>();
            entity.Del<Damage>();
            if (health.hp > damage.amount)
            {
                health.hp -= damage.amount;
            }
            else
            {
                if (entity.Has<Pawn>())
                {
                    ref var pawn = ref entity.Get<Pawn>();
                    Debug.LogFormat("pawn with id {0} is killed", pawn.netFields.ID);
                    NetEntitySyncronizer.instance.EmitDestroy(pawn.netFields.ID);
                }
                else
                {
                    ref var room = ref entity.Get<Room>();
                    room.netFields.ownerID = -1;
                    Debug.LogFormat("room with id {0} is uncaptured", room.netFields.ID);
                    NetEntitySyncronizer.instance.EmitUpdate(room.netFields.ID, new object[] { room });
                }
            }
        }
    }
}
