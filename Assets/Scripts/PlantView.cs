using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantView : MonoBehaviour
{
    [SerializeField]
    private Animator plantAnimator;

    public void Initialize()
    {
        if(plantAnimator == null) 
        { return; }
    }
}
