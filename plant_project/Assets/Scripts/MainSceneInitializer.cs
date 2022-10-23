using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneInitializer : MonoBehaviour
{
    [SerializeField] private GameTimeManagerListener clock;
    [SerializeField] private GameTimeManagerListener backgroundController;

    public void Initialize()
    {
        clock.Initialize();
        backgroundController.Initialize();
    }
}
