using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.UI;
using System.Collections;

public class WeaponEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private Weapon relatedWeapon;
    public void SetUp(Weapon inWeapon)
    {
        relatedWeapon = inWeapon;
        StartCoroutine(waitForEndOfFrame());
    }

    IEnumerator waitForEndOfFrame( )
    {
        yield return new WaitForEndOfFrame();
        textField.text = relatedWeapon.weaponName + "\n" + relatedWeapon.description;
    }

    public static WeaponEntry Create(Transform parent)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/WeaponEntry");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        return go.GetComponent<WeaponEntry>();
    }

    public Weapon GetRelatedWeapon()
    {
        return relatedWeapon;
    }
}
