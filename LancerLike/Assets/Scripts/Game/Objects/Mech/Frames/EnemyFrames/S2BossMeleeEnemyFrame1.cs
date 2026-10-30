using System.Runtime.CompilerServices;
using UnityEngine;

public class S2BossMeleeEnemyFrame : Frame
{
    private void Awake()
    {
        objectSize = BattlefieldObject.Size.Two;
        objectName = "MeleeEnemy";

        structure = 2;
        hp = 15;
        stress = 2;
        heatCapacity = 6;

        armor = 2;
        evasion = 4;
        eDefence = 10;

        sensors = 10;
        techAttack = 0;

        repairCapacity = 1;
        saveTarget = 12;
        speed = 3;
        systemPoints = 0;

    }
    public override void CoreAbility()
    {
        base.CoreAbility();
    }
}
