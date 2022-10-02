
using System;
using UnityEditor.Experimental.GraphView;

public static class Const
{
    // The lifetime in Days
    public static float PlantLifetime = 3;

    // The number of hours to deplete one unit
    public static float HoursToDeplete = 3.0f;

    // The speed at which resource depletes
    // It should go from 1 to 0 in 3 hours. 
    public static float ResourceDepletionPerSecond = 1.0f / (HoursToDeplete * 3600);
    public static float MaxPlantValue = 2.0f;
}
