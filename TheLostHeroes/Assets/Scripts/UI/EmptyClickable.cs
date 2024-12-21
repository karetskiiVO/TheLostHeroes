using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Leopotam.Ecs;
public class EmptyClickable : Clickable
{
    public override void Select() {
        describer?.SetDesctription(new DescriberBehavour.Description{
            entityName = "-",
            entityDescription = "There is nothing to look up",
            actionButtons = new DescriberBehavour.IActionButton[]{}
        });
    }
}