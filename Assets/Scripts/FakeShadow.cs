using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeShadow : MonoBehaviour
{
    [SerializeField]
    private List<SpriteRenderer> sprites;

    [SerializeField]
    private Color darkenedSpriteColor;

    public void ToggleShadow(bool enabled)
    {
        Color spriteColor = enabled ? darkenedSpriteColor : Color.white;
        foreach(var sprite in sprites)
        {
            sprite.color = spriteColor;
        }
    }
}
