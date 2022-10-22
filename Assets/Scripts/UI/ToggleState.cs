using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleState : MonoBehaviour
{
    [SerializeField]
    private Image target;

    [HideInInspector] public Sprite OnSprite;
    [HideInInspector] public Sprite OffSprite;

    private Toggle _toggle;

    public void Init(bool isOn)
    {
        if (target == null)
        { return; }

        _toggle = GetComponent<Toggle>();
        _toggle.isOn = isOn;
        _toggle.onValueChanged.AddListener(SetState);
        SetState(_toggle.isOn);
    }

    public void SetState(bool isOn)
    {
        if (OnSprite == null || OffSprite == null)
        { return; }

        var sprite = (isOn) ? OnSprite : OffSprite;
        target.sprite = sprite;
    }

    public void ForceToggleState(bool isOn)
    {
        _toggle.SetIsOnWithoutNotify(isOn);
        SetState(isOn);
    }
}
