using Actions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuActionStack : ActionStack.ActionBehavior
{
    private void Start()
    {
        ActionStack.Main.PushAction(this);
    }

    public void OnNewGame()
    {
        //ActionStack.Main.RemoveAction(this);
        SceneManager.LoadScene("MechCustomization", LoadSceneMode.Single);
    }

    public void OnTest()
    {
        // create and push the options menu on the main stack!
        Canvas canvas = GetComponentInChildren<Canvas>();
        TestMenu tm = TestMenu.Create(canvas.transform);

        ActionStack.Main.PushAction(tm);
    }

    public void OnQuit()
    {
        {
            #if UNITY_EDITOR
            if (Application.isEditor)
            {
                EditorApplication.ExitPlaymode();
                return;
            }
            #endif
            Application.Quit();
        }
    }

    public override bool IsDone()
    {
        return false;
    }
}