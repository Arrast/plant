using System.Collections;
using UnityEngine;

public class DawnGradient : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer targetSprite;

    [SerializeField]
    private Gradient sunsetGradient;

    [SerializeField]
    private Gradient sunriseGradient;

    public void Move(float percentage, bool goingUp, System.Action onComplete = null)
    {
        if (percentage < 0 || percentage >= 1)
        { return; }

        Gradient gradient = goingUp ? sunriseGradient : sunsetGradient; 
        StartCoroutine(PlayColorGradient(percentage, gradient));
    }

    private IEnumerator PlayColorGradient(float percentage, Gradient gradient)
    {
        float duration = GameTimeManager.RealTimeTransitionDuration;
        float timeElapsed = percentage * duration;
        while (timeElapsed < duration)
        {
            yield return null;
            timeElapsed += Time.deltaTime;
            targetSprite.color = gradient.Evaluate(timeElapsed / duration);
        }
        targetSprite.color = gradient.Evaluate(1);
    }
}
