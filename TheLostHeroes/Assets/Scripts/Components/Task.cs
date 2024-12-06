using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;

public class TaskAttack : IEcsIgnoreInFilter { }
public class TaskDefend : IEcsIgnoreInFilter { }
public class TaskExplore : IEcsIgnoreInFilter { }
public class TaskInteract : IEcsIgnoreInFilter { }

public class Task : MonoBehaviour
{
    public EcsEntity target;
    public int reward;
}
