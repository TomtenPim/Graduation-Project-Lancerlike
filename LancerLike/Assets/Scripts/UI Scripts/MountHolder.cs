using System.CodeDom.Compiler;
using UnityEngine;

public class MountHolder : MonoBehaviour
{
    Mech currentMech;

    public void SetUp(Mech inMech)
    {
        currentMech = inMech;
        GenerateMountSelectors();
    }
    private void GenerateMountSelectors()
    {
        foreach (Mech.WeaponMount weaponMount in currentMech.WeaponMounts)
        {
            MountWeaponSelector.Create(transform, currentMech, weaponMount).UpdateText();
        }
    }
}
