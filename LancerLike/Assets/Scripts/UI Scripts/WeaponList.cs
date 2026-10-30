using UnityEngine;

public class WeaponList : MonoBehaviour
{
    [SerializeField] Weapon[] weaponArray;

    private void Start()
    {
        if (weaponArray.Length <= 0)
        {
            return;
        }

        foreach( Weapon weapon in weaponArray)
        {
            if (weapon == null)
            {
                return;
            }

            WeaponEntry holder = WeaponEntry.Create(this.transform);
            holder.SetUp(weapon);
        }
    }
}
