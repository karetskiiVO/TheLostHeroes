using Leopotam.Ecs;
using Photon.Pun;
using UnityEngine;

public class NetworkedUpdateSystem : IEcsRunSystem
{
    float timer = 1;
    public void Run()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer++;
            PhotonView.Get(NetEntitySyncronizer.instance).RPC("Updated", RpcTarget.All, new object[] { });
        }
    }
}