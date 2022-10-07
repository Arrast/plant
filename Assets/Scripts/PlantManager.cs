using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using versoft.asset_manager;
using versoft.data_model;
using versoft.plant.game_logic;
using versoft.save_data;

public class PlantManager : MonoBehaviour
{
    private List<PlantLogic> _plants = new List<PlantLogic>();
    private List<PlantSavedData> _plantSavedData;
    
    // This will be necessary when we have multiple plants.
    private int selectedPlant = 0;

    // This may be specified somewhere else.
    public float resourceIncreasingMultiplier = 3.0f;


    public float GlobalResourceMultiplier = 50.0f;

    private PlantStat plantStatChanged = PlantStat.None;

    public Action OnTick { get; set; }

    private PlantSavedData CreatePlant(string plantId)
    {
        if (string.IsNullOrEmpty(plantId))
        {
            return null;
        }

        var gameDataProvider = ServiceLocator.Instance.Get<DataModelDatabase>();
        if (gameDataProvider == null)
        {
            return null;
        }

        var dataModel = gameDataProvider.Get<PlantModel>(plantId);
        if (dataModel == null)
        {
            return null;
        }


        long epochTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return new PlantSavedData
        {
            LastTimeUpdated = epochTime,
            PlantId = plantId,
            PlantInstanceId = Guid.NewGuid().ToString(),
            Food = dataModel.Food,
            Light = dataModel.Light,
            Water = dataModel.Water
        };
    }

    private void InitPlants(List<PlantSavedData> savedData)
    {
        if (savedData == null)
        { 
            return; 
        }

        var gameDataProvider = ServiceLocator.Instance.Get<DataModelDatabase>();
        if (gameDataProvider == null)
        {
            return;
        }

        foreach (var plantSavedData in savedData)
        {
            var plantModel = gameDataProvider.Get<PlantModel>(plantSavedData.PlantId);
            if (plantModel == null)
            {
                continue;
            }

            PlantLogic plantLogic = new PlantLogic();
            plantLogic.Init(plantSavedData, plantModel);
            _plants.Add(plantLogic);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var plant in _plants)
        {
            plant.Tick(Time.deltaTime * GlobalResourceMultiplier);
        }

        if(plantStatChanged != PlantStat.None)
        {
            _plants[selectedPlant].ModifyStat(plantStatChanged, Time.deltaTime * resourceIncreasingMultiplier * GlobalResourceMultiplier);
        }

        OnTick?.Invoke();
    }

    public void Init(PlayerSaveData playerSaveData)
    {
        _plantSavedData = new List<PlantSavedData>();
        foreach(var plant in playerSaveData.PlantStates)
        {
            _plantSavedData.Add((PlantSavedData) plant.Clone());
        }
        InitPlants(_plantSavedData);
    }

    public void SetResourceState(PlantStat plantStat, bool increasing)
    {
        if(selectedPlant < 0)
        {
            return;
        }

        plantStatChanged = (increasing) ? plantStat : PlantStat.None;
    }

    public List<PlantSavedData> GetPlants()
    {
        _plantSavedData = new List<PlantSavedData>();
        foreach(var plant in _plants)
        {
            _plantSavedData.Add(plant.GetPlantSaveData());
        }

        return _plantSavedData;
    }

    public PlantLogic GetSelectedPlant()
    {
        if(selectedPlant < 0) { return null; }
        if(selectedPlant >= _plants.Count) { return null; }
        return _plants[selectedPlant];
    }
}
