using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using System;
using System.Text;

public struct RuntimeData {
    //stuff only created in runtime
    
    public EcsEntity envPlayer;
    public EcsEntity Map;

    public RandomConfiguration randomConfiguration;
}
