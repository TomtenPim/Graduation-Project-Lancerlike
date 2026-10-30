using Actions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MountWeaponSelector : MonoBehaviour, IDropHandler
{

    public Mech currentMech;
    [SerializeField] TextMeshProUGUI mountTypeText;
    [SerializeField] TextMeshProUGUI mountCurrentlyEquipedText;
    [SerializeField] TextMeshProUGUI mountCurrentlyEquipedText2;
    Mech.WeaponMount currentMount;
    MountWeaponSelector supporting;
    MountWeaponSelector supportedBy;


    public void UpdateText()
    {
        mountTypeText.text = currentMount.weaponMountType.ToString() + " Mount";
    }

    private void Start()
    {
        UpdateEquipedText();
    }

    public static MountWeaponSelector Create(Transform parent, Mech activeMech, Mech.WeaponMount weaponMount)
    {
        GameObject prefab;
        if (weaponMount.weaponMountType == Mech.WeaponMount.WeaponMountType.Flex)
        {
            prefab = Resources.Load<GameObject>("Prefabs/FlexMountWeaponSelector");
        }
        else
        {
            prefab = Resources.Load<GameObject>("Prefabs/MountWeaponSelector");
        }
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        go.GetComponent<MountWeaponSelector>().currentMech = activeMech;
        go.GetComponent<MountWeaponSelector>().currentMount = weaponMount;
        return go.GetComponent<MountWeaponSelector>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (currentMount.mountedWeapons.Count >= 1 && currentMount.weaponMountType != Mech.WeaponMount.WeaponMountType.Flex)
        {
            TextPopup.Create(transform.parent.transform.parent, Vector3.zero, "Mount already Full");
            return;
        }

        if (currentMount.mountedWeapons.Count >= 2)
        {
            TextPopup.Create(transform.parent.transform.parent, Vector3.zero, "Mount already Full");
            return;
        }

        GameObject dropped = eventData.pointerDrag;
        WeaponEntry we = dropped.GetComponent<WeaponEntry>();



        AddWeaponToMech(we);
        Debug.Log("OnDrop was Called");
    }

    public void OnPressed()
    {
        if (supportedBy != null)
        {
            supportedBy.supporting = null;
            supportedBy.OnPressed();
        }

        ClearMountOfWeapons();
        UpdateEquipedText();
    }

    private void AddWeaponToMech(WeaponEntry inWeaponEntry)
    {
        if (supporting != null)
        {
            TextPopup.Create(transform.parent.transform.parent, Vector3.zero, "Mount already in use");
            return;
        }

        if (inWeaponEntry.GetRelatedWeapon().weaponMountTypeRequred == Mech.WeaponMount.WeaponMountType.SuperHeavy && currentMount.weaponMountType == Mech.WeaponMount.WeaponMountType.Heavy)
        {
            EquipSuperHeavy();
        }
        else
        {
            if (inWeaponEntry.GetRelatedWeapon().weaponMountTypeRequred > currentMount.weaponMountType)
            {
                TextPopup.Create(transform.parent.transform.parent, Vector3.zero, "Mount can't fit the weapon");
                return;
            }
        }

        if (currentMount.mountedWeapons.Count >= 1)
        {
            if (inWeaponEntry.GetRelatedWeapon().weaponMountTypeRequred != Mech.WeaponMount.WeaponMountType.Aux)
            {
                TextPopup.Create(transform.parent.transform.parent, Vector3.zero, "Mount can't fit the weapon");
                return;
            }
            if (currentMount.mountedWeapons[0].weaponMountTypeRequred == Mech.WeaponMount.WeaponMountType.Main)
            {
                TextPopup.Create(transform.parent.transform.parent, Vector3.zero, "Mount already Full");
                return;
            }
        }


        if (currentMount.mountedWeapons.Count >= 1)
        {
            mountCurrentlyEquipedText2.text = inWeaponEntry.GetRelatedWeapon().weaponName;
        }
        else
        {
            mountCurrentlyEquipedText.text = inWeaponEntry.GetRelatedWeapon().weaponName;

            if (inWeaponEntry.GetRelatedWeapon().weaponMountTypeRequred == Mech.WeaponMount.WeaponMountType.Main && mountCurrentlyEquipedText2 != null)
            {
                mountCurrentlyEquipedText2.text = "full";
            }
        }

        currentMount.AddWeaponToMount(currentMech.AddWeapon(inWeaponEntry.GetRelatedWeapon()));
    }

    private void EquipSuperHeavy()
    {
        ActionStack.Main.PushAction(SuperheavyAllocation.Create(transform.parent.transform.parent, currentMech, this));
    }

    public void SetSupprtedBy(MountWeaponSelector supportingMount)
    {
        supportedBy = supportingMount;
        supportingMount.supporting = this;
        supportingMount.UpdateEquipedText();
    }
    public void SetSupprtedBy(Mech.WeaponMount supportingMount)
    {

        MountWeaponSelector[] weaponSelectors = gameObject.transform.parent.GetComponentsInChildren<MountWeaponSelector>();

        foreach (MountWeaponSelector selector in weaponSelectors)
        {
            if (selector.currentMount == supportingMount)
            {

                supportedBy = selector;
                selector.supporting = this;
                selector.UpdateEquipedText();
            }
        }


    }


    private void ClearMountOfWeapons()
    {
        supportedBy = null;

        currentMech.EmptyMount(currentMount);
    }

    private void UpdateEquipedText()
    {
        if (supporting != null)
        {
            mountCurrentlyEquipedText.text = "Supporting";
            return;
        }


        if (currentMount.mountedWeapons.Count <= 0)
        {
            mountCurrentlyEquipedText.text = "empty";
            if (mountCurrentlyEquipedText2 != null)
            {
                mountCurrentlyEquipedText2.text = "empty";
            }
        }
        else
        {
            mountCurrentlyEquipedText.text = currentMount.mountedWeapons[0].weaponName;
            if (currentMount.mountedWeapons.Count > 1 && mountCurrentlyEquipedText2 != null)
            {
                mountCurrentlyEquipedText2.text = currentMount.mountedWeapons[1].weaponName;
            }
        }

    }


}
