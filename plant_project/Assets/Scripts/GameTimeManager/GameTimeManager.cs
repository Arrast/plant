using System;
using System.Collections;
using UnityEngine;

public enum TimeOfDayEnum
{
    Night = 0,
    Dawn,
    Day,
    Dusk
}

public class GameTimeManager : MonoBehaviour
{
    // Real time transition duration
    public static float RealTimeTransitionDuration { get { return GameTimeTransitionDuration / TimeMultiplier; } }

    // Game Time Transition Duration
    public const int GameTimeTransitionDuration = 900;

    // Morning starts at 6 AM
    private const int MorningStartTime = 6;

    // Night starts at 10 PM
    private const int NightStartTime = 22;

    // We tick every 5 seconds
    private const float TickTime = 1; //5;

    // 1 Second in real life is 8 seconds in game time
    public const float TimeMultiplier = 20; //8;

    private TimeOfDayEnum _currentTimeOfDay = TimeOfDayEnum.Day;
    private DateTime _currentDateTime;
    private Action<DateTime> OnTick;
    private Action<TimeOfDayEnum> OnTimeOfDayChanged;

    // Start is called before the first frame update
    void Awake()
    {
        _currentDateTime = DateTime.UtcNow.Date;
        var totalSeconds = DateTime.UtcNow.TimeOfDay.TotalSeconds;
        _currentDateTime = _currentDateTime.AddSeconds(totalSeconds * TimeMultiplier);

        StartCoroutine(Tick());
    }

    private IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(TickTime);
            _currentDateTime = _currentDateTime.AddSeconds(TickTime * TimeMultiplier);
            OnTick?.Invoke(_currentDateTime);

            // Let's calculate the time of day
            var timeOfDay = GetTimeOfDayFromTime();
            if (timeOfDay != _currentTimeOfDay)
            {
                _currentTimeOfDay = timeOfDay;
                OnTimeOfDayChanged?.Invoke(_currentTimeOfDay);
            }
        }
    }

    public TimeOfDayEnum GetTimeOfDayFromTime()
    {
        var hour = _currentDateTime.Hour;
        var elapsedSeconds = _currentDateTime.Second + _currentDateTime.Minute * 60.0f;
        if (hour == NightStartTime && elapsedSeconds < GameTimeTransitionDuration)
        {
            return TimeOfDayEnum.Dusk;
        }
        else if (hour == NightStartTime && elapsedSeconds < GameTimeTransitionDuration)
        {
            return TimeOfDayEnum.Dawn;
        }
        else if (hour >= NightStartTime || hour < MorningStartTime)
        {
            return TimeOfDayEnum.Night;
        }

        return TimeOfDayEnum.Day;
    }

    public void SetOnTickListener(Action<DateTime> onTick)
    {
        OnTick += onTick;
    }

    public void SetOnTimeOfDayChangedListener(Action<TimeOfDayEnum> onTimeChanged)
    {
        OnTimeOfDayChanged += onTimeChanged;
    }

    public DateTime GetTime()
    {
        return _currentDateTime;
    }

    public float GetRemainingRealTimeForPhase()
    {
        if (_currentTimeOfDay == TimeOfDayEnum.Dawn || _currentTimeOfDay == TimeOfDayEnum.Dusk)
        {
            float elapsedGameTime = _currentDateTime.Second + _currentDateTime.Minute * 60.0f;
            return Mathf.Clamp(GameTimeTransitionDuration - elapsedGameTime, 0, GameTimeTransitionDuration) / TimeMultiplier;
        }
        else
        {
            DateTime nextPhaseDateTime = _currentDateTime.Date;
            if (_currentTimeOfDay == TimeOfDayEnum.Night)
            {
                if (_currentDateTime.Hour > MorningStartTime)
                {
                    nextPhaseDateTime.AddDays(1);
                }
                nextPhaseDateTime.AddHours(MorningStartTime);
            }
            else
            {
                nextPhaseDateTime.AddHours(NightStartTime);
            }

            var difference = _currentDateTime - nextPhaseDateTime;
            float remainingDuration = (float)difference.TotalSeconds - GameTimeTransitionDuration;
            return remainingDuration;
        }
    }
}
