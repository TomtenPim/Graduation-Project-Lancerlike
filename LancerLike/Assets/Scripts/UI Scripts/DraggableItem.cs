using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    Transform parentAfterDrag;
    int siblingIndex;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("begin drag");
        transform.SetParent(transform.parent.transform.parent.transform.parent);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("draging");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("end drag");
        transform.SetParent(parentAfterDrag);
        transform.SetSiblingIndex(siblingIndex);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        image.raycastTarget = true;
    }

    private void Start()
    {
        image = GetComponent<Image>();
        parentAfterDrag = transform.parent;
        siblingIndex = transform.GetSiblingIndex();
    }
}
