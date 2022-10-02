using UnityEngine;

namespace versoft.scene_manager
{
    public static class RectTransformExtension
    {
        public static void ResetRectTransform(this RectTransform rectTransform)
        {
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.offsetMin = Vector3.zero;
            rectTransform.offsetMax = Vector3.zero;

            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
        }

        public static void ResetTransform(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
        }
    }
}
