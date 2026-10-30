using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SafeOnLoadObject : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        Debug.Log("SafeOnLoadObject - Start was Called");
    }

    public void SaveObject(GameObject inGameObject)
    {
        inGameObject.transform.parent = this.transform;
    }

    public void LoadObjects(Transform inTransform)
    {
        while (transform.childCount > 0)
        {
            transform.GetChild(0).transform.SetParent(inTransform);
        }

        Destroy(gameObject);
    }
}

