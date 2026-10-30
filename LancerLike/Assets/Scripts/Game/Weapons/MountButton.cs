using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MountButton : MonoBehaviour
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

        if (currentWeaponMount.mountedWeapons[0].hasfired || currentWeaponMount.mountedWeapons[0].canShootAgainWithoutLoading == false)
        {
            this.gameObject.GetComponent<Button>().interactable = false;
        }
    }


    public void onPressed()
    {
        //this.gameObject.GetComponent<Button>().interactable = false;
        this.transform.parent.gameObject.GetComponent<Canvas>().enabled = true;
        PlayerAttackMenu parentMenu = this.transform.parent.gameObject.GetComponent<PlayerAttackMenu>();
        parentMenu.DisableCanvas();
        parentMenu.DisableButtonsIfOutOfActionPoints();
        parentMenu.SetActiveForWeaponButtons(false);
        currentWeaponMount.FireMount();
    }
}
