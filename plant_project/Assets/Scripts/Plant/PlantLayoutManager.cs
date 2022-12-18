using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using versoft.asset_manager;
using versoft.plant.game_logic;


[System.Serializable]
public struct PlantLayoutElementHolder
{
    public string HolderId;
    public PlantLayoutElement HolderLayoutElement;
}

public class PlantLayoutManager : MonoBehaviour
{
    [SerializeField]
    private List<PlantLayoutElementHolder> plantLayoutElements;

    private Dictionary<string, PlantLayoutElement> plantLayoutElementDict;

    public void Initialize()
    {
        if (plantLayoutElements == null)
        { return; }

        var plantManager = ServiceLocator.Instance.Get<PlantManager>();
        if (plantManager == null)
        { return; }

        var plantLayoutSaveData = plantManager.GetLayout();
        if (plantLayoutSaveData == null)
        { return; }

        var plants = plantManager.GetAllPlantLogic();
        if (plants == null)
        {
            UnityEngine.Debug.LogError("There are no plants");
            return;
        }

        InitializePlantLayout(plantLayoutSaveData);
        InitializePlants(plantLayoutSaveData, plants);
    }

    private void InitializePlantLayout(PlantLayoutSaveData plantLayoutSaveData)
    {
        plantLayoutElementDict = plantLayoutElements.ToDictionary(plantLayoutElement => plantLayoutElement.HolderId, plantLayoutElement => plantLayoutElement.HolderLayoutElement);

        // Initialize the Layout Elements
        foreach (var (layoutId, layoutSaveData) in plantLayoutSaveData.Positions)
        {
            if (plantLayoutElementDict.TryGetValue(layoutId, out var layoutElement))
            {
                layoutElement.Initialize(layoutSaveData);
            }
        }
    }

    public PlantView GetPlantViewForPlantId(string plantId)
    {
        foreach(var (_, plantLayoutElement) in plantLayoutElementDict)
        {
            if(plantLayoutElement.Plantviews.TryGetValue(plantId, out var plantView))
            {
                return plantView;
            }
        }
        return null;
    }

    private void InitializePlants(PlantLayoutSaveData plantLayoutSaveData, List<PlantLogic> plants)
    {
        for (int plantCount = 0; plantCount < plants.Count; plantCount++)
        {
            var plantLogic = plants[plantCount];
            CreatePlantFromLogic(plantLogic, plantLayoutSaveData);
        }
    }

    public async void CreatePlantFromLogic(PlantLogic plantLogic, PlantLayoutSaveData plantLayoutSaveData)
    {
        var assetManager = ServiceLocator.Instance.Get<AssetManager>();
        if (assetManager == null)
        { return; }

        PlantView plantPrefab = await assetManager.LoadAsset<PlantView>(Const.PrefabsFolder + "Plant", instantiate: false);
        if (plantPrefab == null)
        { return; }

        var plantSaveData = plantLogic.GetPlantSaveData();

        plantLogic.OnPlantDied += PlantDied;
        plantLogic.OnPlantGrew += PlantGrew;
        plantLogic.OnPlantGeneratedCurrency += PlantGeneratedCurrency;

        var plantInstance = Instantiate(plantPrefab);
        SetPlantPosition(plantSaveData.PlantInstanceId, plantInstance, plantLayoutSaveData);
        plantInstance.Initialize(plantSaveData);
    }

    private void SetPlantPosition(string plantId, PlantView plantInstance, PlantLayoutSaveData plantLayoutSaveData)
    {
        Tuple<string, int> plantTarget = null;
        foreach (var (layoutId, layoutSaveData) in plantLayoutSaveData.Positions)
        {
            var index = layoutSaveData.PlantIds.IndexOf(plantId);
            if (index != -1)
            {
                plantTarget = new Tuple<string, int>(layoutId, index);
                break;
            }
        }

        if (plantTarget == null)
        {
            UnityEngine.Debug.LogError($"We couldn't find an open spot or the existing spot for plant {plantId}");
            return;
        }

        plantLayoutElementDict[plantTarget.Item1].SetPlant(plantId, plantInstance, plantTarget.Item2);
        
    }

    private void PlantGeneratedCurrency(string plantId, int amount)
    {
        // We may want to show something in the views...
        // I'll leave it empty for now.
    }

    private void PlantGrew(string plantId, PlantStage plantStage)
    {
        var plantView = GetPlantViewForPlantId(plantId);
        if(plantView != null)
        {
            plantView.SetPlantStage(plantStage);
        }
    }

    private void PlantDied(string plantId, bool alive)
    {
        var plantView = GetPlantViewForPlantId(plantId);
        if (plantView != null)
        {
            plantView.SetPlantAlive(alive);
        }
    }

    public void ResetPlantWidgets()
    {
        foreach (var (_, plantLayoutElement) in plantLayoutElementDict)
        {
            foreach(var (_,plantView) in plantLayoutElement.Plantviews)
            {
                plantView.ResetButtonState();
            }
        }
    }

    public void UnlockLayout(string layoutId)
    {
        var plantManager = ServiceLocator.Instance.Get<PlantManager>();
        if (plantManager == null)
        { return; }

        var plantLayoutSaveData = plantManager.GetLayout();
        if (plantLayoutSaveData == null)
        { return; }

        if (plantLayoutElementDict.TryGetValue(layoutId, out var plantLayoutElement))
        {
            plantLayoutElement.Initialize(plantLayoutSaveData.Positions[layoutId]);
        }
    }
}
