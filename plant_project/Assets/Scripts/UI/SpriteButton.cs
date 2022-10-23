using System;
using UnityEngine;
using UnityEngine.Events;

public class SpriteButton : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _onClick;

    private bool _clicking = false;
    private bool _interactable = true;
    public void SetInteractable(bool interactable)
    {
        if (interactable != _interactable)
        {
            _clicking = false;
        }
        _interactable = interactable;

    }

    private void OnMouseDown()
    {
        _clicking = _interactable;
    }

    private void OnMouseExit()
    {
        _clicking = false;
    }

    private void OnMouseUpAsButton()
    {
        if (_clicking)
        {
            _clicking = false;
            _onClick?.Invoke();
        }
    }
}
