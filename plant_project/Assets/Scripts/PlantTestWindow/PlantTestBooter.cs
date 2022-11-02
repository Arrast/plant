using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using versoft.asset_manager;

public class PlantTestBooter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ServiceLocator.Instance.Register<AssetManager>();
        var plants = FindObjectsOfType<PlantAnimationTest>();
        if(plants != null)
        {
            foreach(var plant in plants)
            {
                plant.StartTest();
            }
        }
    }
}
