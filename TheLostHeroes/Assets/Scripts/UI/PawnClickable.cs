using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnClickable : Clickable {
    public override void Select () {
        describer?.SetDesctription(new DescriberBehavour.Description{
            entityName = "Sir, Knight",
            entityDescription = "Ready to serve",
            actionButtons = new DescriberBehavour.IActionButton[]{}
        });
    }
}
