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
    private MainSceneInitializer _mainSceneComponents;

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
        _mainSceneComponents = GameObject.FindObjectOfType<MainSceneInitializer>();
        if (_plantLayoutManager == null || _mainSceneComponents == null) 
        { return; }

        _plantLayoutManager.Initialize();
        _mainSceneComponents.Initialize();
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

    public void ToggleResourceOnPlantView(string plantInstanceId, PlantStat plantStat, bool isOn)
    {
        var plantView = _plantLayoutManager.GetPlantViewForPlantId(plantInstanceId);
        if (plantView == null)
        { return; }

        switch (plantStat)
        {
            case PlantStat.Light:
                plantView.SetSunScreen(isOn);
                break;
        }
    }

    public void GivePlant(string randomPlant)
    {
        var plantLogic = _plantManager.CreateNewPlant(randomPlant);
        _plantLayoutManager.CreatePlant(plantLogic);
    }
}
