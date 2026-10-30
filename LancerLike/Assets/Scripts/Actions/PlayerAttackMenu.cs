
using Actions;
using Game;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerAttackMenu : ActionStack.ActionBehavior
{
    private bool m_bIsDone;
    private Mech currentMech;
    [SerializeField] TextMeshProUGUI currentMechName;
    public List<GameObject> createdButtons = new();
    public Button invadeButton;

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
        SetActiveForWeaponButtons(true);
        DisableButtonsForFiredWeapons();
        DisableButtonsIfOutOfActionPoints();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private GameObject CreateButton(Mech.WeaponMount inMount)
    {
        GameObject buttonPrefab = Resources.Load<GameObject>("Prefabs/MountButton");
        GameObject go = Instantiate(buttonPrefab);
        go.transform.SetParent(Battlefield.sm_instance.GetComponentInChildren<PlayerAttackMenu>().transform, false);
        go.GetComponent<MountButton>().CurrentWeaponMount = inMount;
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

    public void DisableButtonsIfOutOfActionPoints()
    {
        if (currentMech.remainingActionPoints <= 0)
        {
            foreach (GameObject go in createdButtons)
            {
                go.GetComponent<Button>().interactable = false;
            }
            invadeButton.interactable = false;
        }
    }

    public void DisableButtonsForFiredWeapons()
    {
        foreach (GameObject go in createdButtons)
        {
            MountButton currentMountButton = go.GetComponent<MountButton>();

            if (currentMountButton.CurrentWeaponMount.mountedWeapons.Count <= 0)
            {
                continue;
            }

            if (!currentMountButton.CurrentWeaponMount.mountedWeapons[0].canShootAgainWithoutLoading || currentMountButton.CurrentWeaponMount.mountedWeapons[0].hasfired || (currentMountButton.CurrentWeaponMount.mountedWeapons[0].isOrdnance && !currentMech.mayFireOrdnance))
            {
                go.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void SetActiveForWeaponButtons(bool inValue)
    {
        foreach (GameObject go in createdButtons)
        {
            go.SetActive(inValue);
        }
    }

    public void OnInvade()
    {
        Debug.Log("OnInvade was Called");
        this.GetComponent<Canvas>().enabled = false;

        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        PlayerInvadeAction pia = PlayerInvadeAction.Create(canvas.transform, currentMech);

        ActionStack.Main.PushAction(pia);
    }

    public void OnGoBack()
    {
        m_bIsDone = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        transform.parent.gameObject.GetComponentInChildren<PlayerActionMenu>().GetComponent<Canvas>().enabled = true;
        Destroy(gameObject);      // commit greater sepuku!
    }

    public void DisableCanvas()
    {
        this.GetComponent<Canvas>().enabled = false;
    }

    public static PlayerAttackMenu Create(Transform parent, Mech activeMech)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerAttackMenu");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        go.GetComponent<PlayerAttackMenu>().currentMech = activeMech;
        return go.GetComponent<PlayerAttackMenu>();
    }
}
