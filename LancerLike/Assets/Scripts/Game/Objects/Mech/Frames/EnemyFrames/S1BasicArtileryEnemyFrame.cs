using System.Runtime.CompilerServices;
using UnityEngine;

public class BasicArtileryEnemyFrame : Frame
{
    private void Awake()
    {
        objectSize = BattlefieldObject.Size.One;
        objectName = "SniperEnemy";

        structure = 1;
        hp = 3;
        stress = 1;
        heatCapacity = 8;

        armor = 0;
        evasion = 8;
        eDefence = 8;

        sensors = 15;
        techAttack = 0;

        repairCapacity = 1;
        saveTarget = 10;
        speed = 3;
        systemPoints = 0;

    }
    public override void CoreAbility()
    {
        base.CoreAbility();
    }
}
