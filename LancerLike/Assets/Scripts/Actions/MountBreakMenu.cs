
using Actions;
using Game;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MountBreakMenu : ActionStack.ActionBehavior
{
    private bool m_bIsDone;
    private Mech currentMech;
    [SerializeField] TextMeshProUGUI currentMechName;
    public List<GameObject> createdButtons = new();

    public override bool IsDone()
    {
        return m_bIsDone;
    }

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);

        currentMechName.text = currentMech.name;
        

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
        GameObject buttonPrefab = Resources.Load<GameObject>("Prefabs/MountBreakButton");
        GameObject go = Instantiate(buttonPrefab);
        go.transform.SetParent(transform, false);
        go.GetComponent<MountBreakButton>().CurrentWeaponMount = inMount;
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
        m_bIsDone = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Destroy(gameObject);      // commit greater sepuku!
    }

    public static MountBreakMenu Create(Transform parent, Mech activeMech)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/MountBreakMenu");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        go.GetComponent<MountBreakMenu>().currentMech = activeMech;
        return go.GetComponent<MountBreakMenu>();
    }
}
