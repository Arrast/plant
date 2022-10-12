using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using versoft.asset_manager;
using versoft.plant.game_logic;

public class GameManager : MonoBehaviour
{
    private PlayerManager _playerManager;
    private PlantManager _plantManager;
    private PlantLayoutManager _plantLayoutManager;
    private CameraManager _cameraManager;

    public void Init()
    {
        _plantManager = ServiceLocator.Instance.Get<PlantManager>();
        _playerManager = ServiceLocator.Instance.Get<PlayerManager>();
        _playerManager.Initialize(); 
        InitializeSceneComponents();
    }

    private void InitializeSceneComponents()
    {
        _cameraManager = gameObject.AddComponent<CameraManager>();
        _plantLayoutManager = GameObject.FindObjectOfType<PlantLayoutManager>();
        if(_plantLayoutManager == null) 
        { return; }

        _plantLayoutManager.Initialize();
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

    public void SelectPlant(string plantId)
    {
        if (string.IsNullOrEmpty(plantId)) 
        { return; }

        var plantView = _plantLayoutManager.GetPlantViewForPlantId(plantId);
        if(plantView == null)
        { return; }

        _plantManager.SelectPlant(plantId);
        _cameraManager.ZoomToGameObject(plantView.transform.parent.gameObject, Vector2.up);
    }

    public void DeselectPlant()
    {
        _plantManager.DeselectPlant();
        _cameraManager.ResetCameraValues();
        _plantLayoutManager.ResetPlantWidgets();
    }
}
