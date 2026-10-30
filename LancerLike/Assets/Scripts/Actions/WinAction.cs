
using Actions;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WinAction : ActionStack.ActionBehavior
{
    private bool m_bIsDone;

    #region Properties

    #endregion

    public override bool IsDone()
    {
        return m_bIsDone;
    }

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public void OnReturnToMenu()
    {
        m_bIsDone = true;
    }

    public override void OnEnd()
    {

        base.OnEnd();
        Destroy(ActionStack.Main.gameObject);
        Destroy(gameObject);      // commit greater sepuku!

        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }

    public static WinAction Create(Transform parent)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/WinScreen");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        return go.GetComponent<WinAction>();
    }
}
