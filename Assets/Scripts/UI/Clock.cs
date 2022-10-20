using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clock : GameTimeManagerListener
{
    [SerializeField] private Transform hourHand;
    [SerializeField] private Transform minuteHand;
    [SerializeField] private TMP_Text timeLabel;
    
    public override void TimeManagerTicked(DateTime currentTime)
    {
        if(hourHand != null)
        {
            hourHand.localRotation = Quaternion.Euler(Vector3.back * currentTime.Hour * 30);
        }

        if(minuteHand != null)
        {

            minuteHand.localRotation = Quaternion.Euler(Vector3.back * currentTime.Minute * 6);
        }

        if(timeLabel != null)
        {
            timeLabel.text = $"{currentTime.Hour.ToString("00")}:{currentTime.Minute.ToString("00")}";
        }
    }
}
