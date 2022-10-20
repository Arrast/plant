using System;
using System.Collections.Generic;
using UnityEngine;
using versoft.asset_manager;

public class GameTimeBackgroundController : GameTimeManagerListener
{
    [Serializable]
    private class TimeBackgroundData
    {
        public TimeOfDayEnum Time;
        public Color BackgroundColor;
        public List<GameObject> ExtraObjects;
    }

    [SerializeField]
    private List<TimeBackgroundData> timeBackgroundData;

    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private DawnGradient dawnAnimation;

    private TimeOfDayEnum _currentTimeOfDay = TimeOfDayEnum.Day;

    protected override void FirstTick()
    {
        // We need special logic here
        float percentage = 0.0f;
        var timeOfDay = gameTimeManager.GetTimeOfDayFromTime();
        if ((timeOfDay == TimeOfDayEnum.Dusk || timeOfDay == TimeOfDayEnum.Dawn) && dawnAnimation != null)
        {
            var remainingRealTime = gameTimeManager.GetRemainingRealTimeForPhase();
            if (remainingRealTime < GameTimeManager.RealTimeTransitionDuration)
            {
                percentage = 1 - remainingRealTime / GameTimeManager.RealTimeTransitionDuration;
            }
        }
        SelectTimeOfDay(timeOfDay, true, percentage);
    }

    public override void TimeManagerTicked(DateTime currentTime)
    {
        var timeOfDay = gameTimeManager.GetTimeOfDayFromTime();
        SelectTimeOfDay(timeOfDay);
    }

    private void SelectTimeOfDay(TimeOfDayEnum timeOfDay, bool force = false, float animationPercentageStarted = 0)
    {
        if (timeOfDay == _currentTimeOfDay && !force)
        { return; }

        _currentTimeOfDay = timeOfDay;
        foreach (var data in timeBackgroundData)
        {
            if (data.Time == timeOfDay)
            {
                sprite.color = data.BackgroundColor;
            }

            if (data.ExtraObjects == null)
            { continue; }

            foreach (var objectToToggle in data.ExtraObjects)
            {
                objectToToggle.SetActive(data.Time == timeOfDay);
            }
        }

        PlayTimeOfDayAnimation(animationPercentageStarted);
    }

    private void PlayTimeOfDayAnimation(float animationPercentageStarted)
    {
        if (dawnAnimation == null)
        { return; }

        if (_currentTimeOfDay == TimeOfDayEnum.Dawn)
        {
            dawnAnimation.SafeSetActive(true);
            dawnAnimation.Move(animationPercentageStarted, true, () =>
            {
                dawnAnimation.SafeSetActive(false);
            });
        }
        else if (_currentTimeOfDay == TimeOfDayEnum.Dusk)
        {
            dawnAnimation.SafeSetActive(true);
            dawnAnimation.Move(animationPercentageStarted, false, () =>
            {
                dawnAnimation.SafeSetActive(false);
            });
        }
        else
        {
            dawnAnimation.SafeSetActive(false);
        }
    }
}
