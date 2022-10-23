using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private const float zoomSize = 2.0f;
    private float defaultCameraSize = 2.0f;

    private void Awake()
    {
        defaultCameraSize = Camera.main.orthographicSize;
    }

    public void ZoomToGameObject(GameObject target, Vector2 offset = new Vector2())
    {
        Vector2 targetPosition = target.transform.localPosition;
        targetPosition += offset;
        Camera.main.orthographicSize = zoomSize;
        Camera.main.transform.localPosition = (Vector3)targetPosition + Vector3.back * 10;
    }

    public void ResetCameraValues()
    {
        Camera.main.orthographicSize = defaultCameraSize;
        Camera.main.transform.localPosition = Vector3.back * 10;
    }
}
