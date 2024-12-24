using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;
public struct CombatSystem : IEcsRunSystem
{
    EcsFilter<AttackActor> actorFilter;

    public void Run()
    {
        foreach (int i in actorFilter)
        {
            var actor = actorFilter.Get1(i);
            var entity = actorFilter.GetEntity(i);
            entity.Del<AttackActor>();
            if (NetEntitySyncronizer.Alive(actor.targetID))
            {
                var targetEntity = NetEntitySyncronizer.GetEntity(actor.targetID);
                if (targetEntity.Has<Damage>())
                {
                    targetEntity.Get<Damage>().amount += entity.Get<Attack>().atk;
                }
                else
                {
                    targetEntity.Get<Damage>().amount = entity.Get<Attack>().atk;
                }
            }
        }
    }
}
