using System.Runtime.CompilerServices;
using UnityEngine;

public class BasicRangedEnemyFrame : Frame
{
    private void Awake()
    {
        objectSize = BattlefieldObject.Size.One;
        objectName = "RangedEnemy";

        structure = 1;
        hp = 8;
        stress = 1;
        heatCapacity = 8;

        armor = 0;
        evasion = 8;
        eDefence = 8;

        sensors = 10;
        techAttack = 0;

        repairCapacity = 1;
        saveTarget = 10;
        speed = 4;
        systemPoints = 0;

    }
    public override void CoreAbility()
    {
        base.CoreAbility();
    }
}
