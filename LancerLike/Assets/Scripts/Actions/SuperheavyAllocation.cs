
using Actions;
using Game;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SuperheavyAllocation : ActionStack.ActionBehavior
{
    private bool m_bIsDone;
    private Mech currentMech;
    private MountWeaponSelector usedHeavyMount;
    public List<GameObject> createdButtons = new();

    public override bool IsDone()
    {
        return m_bIsDone;
    }

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);

        if (bFirstTime)
        {
            foreach (Mech.WeaponMount mount in currentMech.WeaponMounts)
            {
                createdButtons.Add(CreateButton(mount));
            }
            SpaceMountButtons();
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }


    private GameObject CreateButton(Mech.WeaponMount inMount)
    {
        GameObject buttonPrefab = Resources.Load<GameObject>("Prefabs/SuperheavyButton");
        GameObject go = Instantiate(buttonPrefab);
        go.transform.SetParent(transform, false);
        go.GetComponent<SuperheavyMountButton>().CurrentWeaponMount = inMount;
        return go;
    }

    private void SpaceMountButtons()
    {
        int i = 1;
        foreach (GameObject go in createdButtons)
        {
            if (i == 1)
            {
                go.transform.position = new Vector3(go.transform.position.x - 100 * (createdButtons.Count - 1), go.transform.position.y, go.transform.position.z);
            }

            if (i == createdButtons.Count)
            {
                go.transform.position = new Vector3(go.transform.position.x + 100 * (createdButtons.Count - 1), go.transform.position.y, go.transform.position.z);
            }
            i++;
        }
    }

    public void OnGoBack()
    {
        usedHeavyMount.OnPressed();

        m_bIsDone = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Destroy(gameObject);      // commit greater sepuku!
    }

    public void EquipWeapon(Mech.WeaponMount inMount)
    {
        usedHeavyMount.SetSupprtedBy(inMount);
        m_bIsDone = true;
    }

    public static SuperheavyAllocation Create(Transform parent, Mech activeMech, MountWeaponSelector inUsedHeavyMount)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/SuperheavyAdditionalMountSelection");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        go.GetComponent<SuperheavyAllocation>().currentMech = activeMech;
        go.GetComponent<SuperheavyAllocation>().usedHeavyMount = inUsedHeavyMount;
        return go.GetComponent<SuperheavyAllocation>();
    }
}
