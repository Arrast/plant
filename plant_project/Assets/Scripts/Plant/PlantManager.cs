using System;
using System.Collections.Generic;
using UnityEngine;
using versoft.asset_manager;
using versoft.data_model;
using versoft.plant.game_logic;

public class PlantManager : MonoBehaviour
{
    private List<PlantLogic> _plants = new List<PlantLogic>();
    private List<PlantSaveData> _plantSavedData;
    private PlantLayoutSaveData _plantLayoutSaveData;

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

    public PlantLogic CreateNewPlant(string plantId)
    {
        var plantSavedData = CreatePlant(plantId);
        if (plantSavedData == null)
        { return null; }

        var plantLogic = CreatePlantLogic(plantSavedData);
        if (plantLogic == null)
        { return null; }

        AddNewPlantToFirstEmptyLayoutSlot(plantSavedData.PlantInstanceId);

        _plantSavedData.Add(plantSavedData);
        _plants.Add(plantLogic);

        return plantLogic;
    }

    private void AddNewPlantToFirstEmptyLayoutSlot(string plantInstanceId)
    {
        var slot = GetFirstEmptyLayoutSlot();
        if (slot == null)
        { return; }

        _plantLayoutSaveData.Positions[slot.Item1].PlantIds.Add(plantInstanceId);
    }

    private Tuple<string, int> GetFirstEmptyLayoutSlot()
    {
        if (_plantLayoutSaveData == null)
        {
            UnityEngine.Debug.LogError("We don't have a Layout Save Data... That shouldn't be happening");
            return null;
        }

        foreach (var (positionId, positionSaveData) in _plantLayoutSaveData.Positions)
        {
            if (!positionSaveData.Unlocked)
            {
                continue;
            }

            if (positionSaveData.PlantIds.Count < positionSaveData.MaxNumberOfPlants)
            {
                return new Tuple<string, int>(positionId, positionSaveData.PlantIds.Count);
            }
        }

        return null;
    }

    private PlantSaveData CreatePlant(string plantId)
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
        return new PlantSaveData
        {
            LastTimeUpdated = epochTime,
            PlantId = plantId,
            PlantInstanceId = Guid.NewGuid().ToString(),
            Food = dataModel.Food,
            Light = dataModel.Light,
            Water = dataModel.Water
        };
    }

    private PlantLogic CreatePlantLogic(PlantSaveData plantSavedData)
    {
        if (plantSavedData == null)
        { return null; }

        var gameDataProvider = ServiceLocator.Instance.Get<DataModelDatabase>();
        if (gameDataProvider == null)
        { return null; }

        var plantModel = gameDataProvider.Get<PlantModel>(plantSavedData.PlantId);
        if (plantModel == null)
        { return null; }

        PlantLogic plantLogic = new PlantLogic();
        plantLogic.Init(plantSavedData, plantModel);
        return plantLogic;
    }

    private void AddPlantLogicToList(PlantLogic plantLogic)
    {
        if (plantLogic == null)
        { return; }

        _plants.Add(plantLogic);
    }

    private void InitPlants(List<PlantSaveData> savedData)
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
            PlantLogic plantLogic = CreatePlantLogic(plantSavedData);
            AddPlantLogicToList(plantLogic);
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
        _plantLayoutSaveData = (PlantLayoutSaveData)playerSaveData.PlantLayout.Clone();
        _plantSavedData = new List<PlantSaveData>();
        foreach (var plant in playerSaveData.PlantStates)
        {
            _plantSavedData.Add((PlantSaveData)plant.Clone());
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

    public List<PlantSaveData> GetPlants()
    {
        _plantSavedData = new List<PlantSaveData>();
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

    public PlantLayoutSaveData GetLayout()
    {
        return _plantLayoutSaveData;
    }

    public void UnlockLayout(string layoutId)
    {
        if (_plantLayoutSaveData.Positions.TryGetValue(layoutId, out var layoutSaveData))
        {
            layoutSaveData.Unlocked = true;
        }
    }
}
