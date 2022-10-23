using Mono.Cecil.Cil;
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
    private int selectedPlant = -1;

    // This may be specified somewhere else.
    public float resourceIncreasingMultiplier = 1000.0f;

    public float GlobalResourceMultiplier = 1.0f;

    private PlantStat plantStatChanged = PlantStat.None;

    public PlantLogic SelectedPlant
    {
        get
        {
            if (selectedPlant < 0) { return null; }
            if (selectedPlant >= _plants.Count) { return null; }
            return _plants[selectedPlant];
        }
    }

    public Action OnTick { get; set; }
    public Action<PlantLogic> OnPlantSelected { get; set; }

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

        if (plantStatChanged != PlantStat.None)
        {
            SelectedPlant?.ModifyStat(plantStatChanged, Time.deltaTime * resourceIncreasingMultiplier * GlobalResourceMultiplier * Const.ResourceDepletionPerSecond);
        }

        OnTick?.Invoke();
    }

    public void LoadPlantsFromSaveData(PlayerSaveData playerSaveData)
    {
        _plantSavedData = new List<PlantSavedData>();
        foreach (var plant in playerSaveData.PlantStates)
        {
            _plantSavedData.Add((PlantSavedData)plant.Clone());
        }
        InitPlants(_plantSavedData);
    }

    public void SetResourceState(PlantStat plantStat, bool increasing)
    {
        if (selectedPlant < 0)
        {
            return;
        }

        plantStatChanged = (increasing) ? plantStat : PlantStat.None;
    }

    public List<PlantSavedData> GetPlants()
    {
        _plantSavedData = new List<PlantSavedData>();
        foreach (var plant in _plants)
        {
            _plantSavedData.Add(plant.GetPlantSaveData());
        }

        return _plantSavedData;
    }

    public List<PlantLogic> GetAllPlantLogic()
    {
        return _plants;
    }

    public void SelectPlant(string plantId)
    {
        for (int i = 0; i < _plantSavedData.Count; i++)
        {
            if (_plantSavedData[i].PlantInstanceId == plantId)
            {
                selectedPlant = i;
            }
        }

        OnPlantSelected?.Invoke(SelectedPlant);
    }

    public void DeselectPlant()
    {
        selectedPlant = -1;

        OnPlantSelected?.Invoke(SelectedPlant);
    }

    public void ToggleResourceModifier(PlantStat plantStat, bool isOn)
    {
        if (isOn)
        {
            var modifierFunction = GetModifierForStat(plantStat);
            if (modifierFunction != null)
            {
                SelectedPlant?.AddModifierForStat(plantStat, modifierFunction);
            }
        }
        else
        {
            SelectedPlant?.RemoveModifierForStat(plantStat);
        }
    }

    public Func<float> GetModifierForStat(PlantStat plantStat)
    {
        switch (plantStat)
        {
            case PlantStat.Light:
                {
                    return PlantLightModifier;
                }
            default:
                return null;
        }
    }

    public float PlantLightModifier()
    {
        var gameTimeManager = ServiceLocator.Instance.Get<GameTimeManager>();
        if (gameTimeManager.GetTimeOfDayFromTime() == TimeOfDayEnum.Day)
        {
            return 0.5f;
        }
        return 1.0f;
    }
}