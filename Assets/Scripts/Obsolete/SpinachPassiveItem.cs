using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("deprecated")]
public class SpinachPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMight *= 1 + passiveItemData.Multiplier / 100f;
    }   
}
