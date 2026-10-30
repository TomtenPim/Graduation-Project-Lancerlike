using System.Runtime.CompilerServices;
using UnityEngine;

public class S2TargetDummy : Frame
{
    private void Awake()
    {
        objectSize = BattlefieldObject.Size.Two;
        objectName = "Everest";

        structure = 1;
        hp = 1;
        stress = 1;
        heatCapacity = 1;

        armor = 0;
        evasion = 0;
        eDefence = 0;

        sensors = 20;
        techAttack = 0;

        repairCapacity = 5;
        saveTarget = 10;
        speed = 4;
        systemPoints = 6;
        weaponMounts.Add(new Mech.WeaponMount(Mech.WeaponMount.WeaponMountType.Main));      
    }
}
