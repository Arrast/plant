using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DawnAnimation : MonoBehaviour
{
    [SerializeField]
    private GameTimeManager timeManager;

    [SerializeField]
    private float step = 0;

    [SerializeField]
    private float yPosition = 10;

    // Real time duration in seconds.
    public float Duration { get; private set; } = 120;


    public void Move(float percentage, bool goingUp, System.Action onComplete = null)
    {
        if(percentage < 0 || percentage >= 1) 
        { return; }

        float actualDuration = Duration * (1 - percentage);
        float initialPosition = -yPosition + yPosition * 2 * percentage;
        StartCoroutine(MoveInternal(initialPosition, actualDuration, goingUp, onComplete)); 
    }


    private IEnumerator MoveInternal(float initialYPosition, float duration, bool goingUp, System.Action onComplete = null)
    {
        float timeStep = duration / (Mathf.Abs(initialYPosition) + Mathf.Abs(yPosition)) * step;
        float elapsedTime = 0;
        Vector3 vectorBase = (goingUp) ? Vector3.up : Vector3.down;


        transform.localPosition = vectorBase * initialYPosition;

        while(elapsedTime < duration)
        {
            yield return new WaitForSeconds(timeStep);
            elapsedTime += timeStep;
            transform.localPosition += vectorBase * step;
        }

        transform.localPosition = vectorBase * -initialYPosition;
        onComplete?.Invoke();
    }
}
