using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Toolbar : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private MenuWindow menuWindow;

    bool selected = false;
    Vector3 mouseOriginalPosition;
    Vector3 transformOriginalPosition;

    private void Update()
    {
        if (!selected)
            return;

        Vector3 mouseCurrentPosition = Input.mousePosition;

        menuWindow.transform.position = transformOriginalPosition + (mouseCurrentPosition - mouseOriginalPosition);
    }

    private void OnMouseUp()
    {
        selected = false;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        selected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        selected = true;
        mouseOriginalPosition = Input.mousePosition;
        transformOriginalPosition = menuWindow.transform.position;
    }
}
