using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;
using versoft.asset_manager;

public class GameTimeManagerListener : MonoBehaviour
{
    protected GameTimeManager gameTimeManager;

    public virtual void Initialize()
    {
        gameTimeManager = ServiceLocator.Instance.Get<GameTimeManager>();
        if (gameTimeManager == null)
        { return; }

        // First Tick
        FirstTick();

        // Subscribe to the tick event.
        gameTimeManager.SetOnTickListener(TimeManagerTicked);
    }

    protected virtual void FirstTick()
    {
        var currentTime = gameTimeManager.GetTime();
        TimeManagerTicked(currentTime);
    }

    // Update is called once per frame
    public virtual void TimeManagerTicked(DateTime currentTime)
    {
        
    }
}
