using System.Runtime.CompilerServices;
using UnityEngine;

public class Everest : Frame
{
    private void Awake()
    {
        objectSize = BattlefieldObject.Size.One;
        objectName = "Everest";

        structure = 4;
        hp = 10;
        stress = 4;
        heatCapacity = 8;

        armor = 0;
        evasion = 8;
        eDefence = 8;

        sensors = 10;
        techAttack = 0;

        repairCapacity = 5;
        saveTarget = 10;
        speed = 4;
        systemPoints = 6;
        weaponMounts.Add(new Mech.WeaponMount(Mech.WeaponMount.WeaponMountType.Main));
        weaponMounts.Add(new Mech.WeaponMount(Mech.WeaponMount.WeaponMountType.Flex));
        weaponMounts.Add(new Mech.WeaponMount(Mech.WeaponMount.WeaponMountType.Heavy));        
    }
    public override void CoreAbility()
    {
        base.CoreAbility();
    }
}
