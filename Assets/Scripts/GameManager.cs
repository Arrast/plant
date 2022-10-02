using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using versoft.asset_manager;
using versoft.plant.game_logic;

public class GameManager : MonoBehaviour
{
    private PlayerManager _playerManager;
    private PlantManager _plantManager;

    public void Init()
    {
        _plantManager = ServiceLocator.Instance.Get<PlantManager>();
        _playerManager = ServiceLocator.Instance.Get<PlayerManager>();
        _playerManager.Initialize();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            SetResourceState(PlantStat.Water, true);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            SetResourceState(PlantStat.Water, false);
        }
    }

    // We save when we close the game.
    private void OnApplicationQuit()
    {
        _playerManager.SaveToFile();
    }

    public void SetResourceState(PlantStat plantStat, bool increasing)
    {
        if(_plantManager == null) 
        { return; }

        _plantManager.SetResourceState(plantStat, increasing);
    }
}
