using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using System;
using System.Text;
using UnityEngine;

public struct RuntimeData
{
    public int id;
    //stuff only created in runtime
    public EcsEntity map;

    public RandomConfiguration randomConfiguration;
}
