using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SuperheavyMountButton : MonoBehaviour
{
    
    Mech.WeaponMount currentWeaponMount;
    [SerializeField] TextMeshProUGUI mountTypeText;
    [SerializeField] TextMeshProUGUI weaponName;

    public Mech.WeaponMount CurrentWeaponMount
    {
        get { return currentWeaponMount; }
        set { currentWeaponMount = value; }
    }

    public void Start()
    {
        mountTypeText.text = currentWeaponMount.GetMountTypeName();

        if (currentWeaponMount.mountedWeapons.Count <= 0)
        {
            if(currentWeaponMount.weaponMountType >= Mech.WeaponMount.WeaponMountType.Main)
            {
                weaponName.text = "avalible";
                this.gameObject.GetComponent<Button>().interactable = true;

                return;
            }
            else
            {
                weaponName.text = "too small";
                return;
            }

        }

        if (currentWeaponMount.mountedWeapons != null && currentWeaponMount.mountedWeapons.Count > 0)
        {
            weaponName.text = "full";
        }
    }


    public void onPressed()
    {
        //this.gameObject.GetComponent<Button>().interactable = false;
        Debug.Log("SuperheavyMountButton - onPressed was called");
        this.transform.parent.gameObject.GetComponent<SuperheavyAllocation>().EquipWeapon(currentWeaponMount);
    }
}
