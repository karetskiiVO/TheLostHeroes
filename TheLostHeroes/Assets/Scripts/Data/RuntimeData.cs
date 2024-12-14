using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using System;
using System.Text;
using UnityEngine;

public struct RuntimeData {
    //stuff only created in runtime
    
    public EcsEntity envPlayer;
    public EcsEntity map;

    public RandomConfiguration randomConfiguration;
}
