using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MountBreakButton : MonoBehaviour
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
            weaponName.text = "empty";
            this.gameObject.GetComponent<Button>().interactable = false;

            return;
        }

        if (currentWeaponMount.mountedWeapons != null && currentWeaponMount.mountedWeapons.Count > 0)
        {
            weaponName.text = currentWeaponMount.mountedWeapons[0].weaponName;
        }
    }
    public void onPressed()
    {
        currentWeaponMount.EmptyMount();
        MountBreakMenu parentMenu = this.transform.parent.gameObject.GetComponent<MountBreakMenu>();
        parentMenu.OnGoBack();
    }
}
