using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("deprecated")]
public class WingsPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMoveSpeed *= 1 + passiveItemData.Multiplier / 100f;
    }
}
