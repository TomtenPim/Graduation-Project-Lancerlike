using System.Runtime.CompilerServices;
using UnityEngine;

public class MediumMeleeEnemyFrame : Frame
{
    private void Awake()
    {
        objectSize = BattlefieldObject.Size.One;
        objectName = "MeleeEnemy";

        structure = 1;
        hp = 12;
        stress = 2;
        heatCapacity = 6;

        armor = 2;
        evasion = 6;
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
