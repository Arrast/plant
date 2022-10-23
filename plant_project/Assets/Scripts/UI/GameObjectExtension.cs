using UnityEngine;

public static class GameObjectExtension
{
    public static void SafeSetActive(this GameObject gameObject, bool active)
    {
        if(gameObject == null) { return; }
        gameObject.SetActive(active);
    }

    public static void SafeSetActive(this Component component, bool active)
    {
        if(component == null) { return; }
        component.gameObject.SetActive(active);
    }
}
