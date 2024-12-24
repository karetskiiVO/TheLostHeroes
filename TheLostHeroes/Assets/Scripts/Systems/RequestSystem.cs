using UnityEngine;
using Leopotam.Ecs;
using Photon.Pun;

public struct RequestSystem : IEcsRunSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    private EcsFilter<RecruitRequest, Barrack, Room> recruitFilter;
    private EcsFilter<UpgradeRequest, Room> upgradeFilter;

    public void Run()
    {
        foreach (var idx in recruitFilter)
        {
            var roomComponent = recruitFilter.Get3(idx);
            if (roomComponent.netFields.ownerID != PhotonNetwork.LocalPlayer.ActorNumber)
                continue;
            if (MoneyCounter.StaticDecreaseIfHas(220))
            {
                makePawn(roomComponent.netFields.ownerID, 1, new Vector2(roomComponent.netFields.posx, roomComponent.netFields.posy));
            }
            else Debug.Log("Нужно больше золота...");
        }
        foreach (var idx in upgradeFilter)
        {
            //TODO:upgrade?
        }

    }

    public static void makePawn(int owner, int level, Vector2 pos)
    {
        // TODO: учесть уровень
        var pawn = new Pawn();
        pawn.netFields.speed = 0.01f;
        pawn.netFields.x = pos.x;
        pawn.netFields.y = pos.y;
        pawn.netFields.ownerID = owner;
        pawn.netFields.taskID = -1;
        pawn.netFields.ID = NetEntitySyncronizer.instance.nextID;
        var health = new Health
        {
            hp = 5,
            maxhp = 5,
        };
        var attack = new Attack
        {
            atk = 1,
        };
        var money = new Money
        {
            money = 0,
        };
        var state = new PawnIdle();

        NetEntitySyncronizer.instance.EmitCreate(NetEntitySyncronizer.instance.nextID++, new object[] { pawn, health, attack, money, state });
    }
}