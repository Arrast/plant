using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using versoft.asset_manager;
using versoft.scene_manager;

public class DraggableElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Vector3 scaleFactor = Vector3.one;

    [SerializeField]
    private string targetTag;

    public System.Action WidgetEnteredTarget;
    public System.Action WidgetLeftTarget;

    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Vector2 _originalPosition;
    private Vector3 _originalScale;

    void Awake()
    {
        _rectTransform = transform as RectTransform;
        _canvas = ServiceLocator.Instance.Get<WindowManager>().MainCanvas;
        _originalPosition = _rectTransform.anchoredPosition;
        _originalScale = _rectTransform.localScale;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        _rectTransform.localScale = Vector3.Scale(_originalScale, scaleFactor);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition = _originalPosition; 
        _rectTransform.localScale = _originalScale;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == targetTag)
        {
            WidgetEnteredTarget?.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == targetTag)
        {
            WidgetLeftTarget?.Invoke();
        }
    }
}
