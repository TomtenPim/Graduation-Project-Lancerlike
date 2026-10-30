
using Actions;
using UnityEngine;


public class TestMenu : ActionStack.ActionBehavior
{
    private bool m_bIsDone;

    #region Properties

    #endregion

    public override bool IsDone()
    {
        return m_bIsDone;
    }

    public void OnGoBack()
    {
        m_bIsDone = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Destroy(gameObject);      // Plan C!
    }

    public static TestMenu Create(Transform parent)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/TestMenu");
        GameObject go = Instantiate(prefab, parent);
        return go.GetComponent<TestMenu>();
    }
}
