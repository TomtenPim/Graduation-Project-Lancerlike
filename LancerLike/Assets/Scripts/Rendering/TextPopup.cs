using Game;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TextPopup : MonoBehaviour
{
    string stringToDisplay;
    [SerializeField] TextMeshProUGUI textField;
    Vector3 position;

    protected void Start()
    {
        //this is the ui element
        RectTransform UI_Element = this.GetComponent<RectTransform>();
        //first you need the RectTransform component of your canvas
        RectTransform CanvasRect = this.GetComponentInParent<RectTransform>();
        //then you calculate the position of the UI element

        Vector2 ViewportPosition = new();
        if (SceneManager.GetActiveScene().name == "HexGrids")
        {
            ViewportPosition = Battlefield.sm_instance.GetComponentInChildren<Camera>().WorldToViewportPoint(position);

        }
        else
        {
            ViewportPosition = transform.root.GetComponentInChildren<Camera>().WorldToViewportPoint(position);
        }
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ViewportPosition.x * CanvasRect.sizeDelta.x,
        ViewportPosition.y * CanvasRect.sizeDelta.y);
        UI_Element.anchoredPosition = WorldObject_ScreenPosition;

        textField.text = stringToDisplay;
        StartCoroutine(FadeOverTime());
    }


    public static void Create(Transform parent, Vector3 inPosition, string inString)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/TextPopup");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        TextPopup thisPopup = go.GetComponent<TextPopup>();
        thisPopup.position = inPosition;
        thisPopup.stringToDisplay = inString;
    }

    public static void Create(Transform parent, Vector3 inPosition, string inString, Color textColor)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/TextPopup");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        TextPopup thisPopup = go.GetComponent<TextPopup>();
        thisPopup.position = inPosition;
        thisPopup.stringToDisplay = inString;
        thisPopup.textField.color = textColor;
    }

    public static void Create(Transform parent, Vector3 inPosition, float inFloat)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/TextPopup");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        TextPopup thisPopup = go.GetComponent<TextPopup>();
        thisPopup.position = inPosition;
        thisPopup.stringToDisplay = inFloat.ToString();
    }

    public static void Create(Transform parent, Vector3 inPosition, float inFloat, Color textColor)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/TextPopup");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        TextPopup thisPopup = go.GetComponent<TextPopup>();
        thisPopup.position = inPosition;
        thisPopup.stringToDisplay = inFloat.ToString();
        thisPopup.textField.color = textColor;
    }

    IEnumerator FadeOverTime()
    {
        Color color = textField.color;
        for (float alpha = 1f; alpha >= 0; alpha -= 0.012f)
        {
            color.a = alpha;
            textField.color = color;

            yield return new WaitForFixedUpdate();
        }
        Destroy(this.gameObject);
    }
}
