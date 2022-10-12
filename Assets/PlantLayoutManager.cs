using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using versoft.asset_manager;
using versoft.plant.game_logic;

public class PlantLayoutManager : MonoBehaviour
{
    [SerializeField]
    private List<Transform> plantPositions;

    [SerializeField]
    private Transform plantHolderBackground;

    private Dictionary<string, PlantView> _plantViews = new Dictionary<string, PlantView>();
    public void Initialize()
    {
        if (plantPositions == null)
        { return; }

        var plantManager = ServiceLocator.Instance.Get<PlantManager>();
        if (plantManager == null)
        { return; }

        var plants = plantManager.GetAllPlantLogic();
        if (plants == null || plants.Count > plantPositions.Count)
        {
            UnityEngine.Debug.LogError("There are more plants that plant positions");
            return;
        }

        InitializePlants(plants);
    }

    public PlantView GetPlantViewForPlantId(string plantId)
    {
        if (_plantViews.TryGetValue(plantId, out var plantView))
        {
            return plantView;
        }
        return null;
    }

    private async void InitializePlants(List<PlantLogic> plants)
    {
        var assetManager = ServiceLocator.Instance.Get<AssetManager>();
        if (assetManager == null)
        { return; }

        PlantView plantPrefab = await assetManager.LoadAsset<PlantView>(Const.PrefabsFolder + "Plant", instantiate: false);
        if (plantPrefab == null)
        { return; }


        int plantCount = 0;
        for (; plantCount < plants.Count; plantCount++)
        {
            var plantLogic = plants[plantCount];
            var plantSaveData = plantLogic.GetPlantSaveData();

            plantLogic.OnPlantDied += PlantDied;
            plantLogic.OnPlantGrew += PlantGrew;

            plantPositions[plantCount].SafeSetActive(true);
            var plantInstance = Instantiate(plantPrefab);
            plantInstance.transform.SetParent(plantPositions[plantCount]);
            plantInstance.transform.localPosition = Vector3.zero;
            plantInstance.transform.localRotation = Quaternion.identity;
            plantInstance.transform.localScale = Vector3.one;
            plantInstance.Initialize(plantSaveData);

            _plantViews.Add(plantSaveData.PlantInstanceId, plantInstance);
        }

        for (; plantCount < plantPositions.Count; plantCount++)
        {
            plantPositions[plantCount].SafeSetActive(false);
        }
    }

    private void PlantGrew(string plantId, PlantStage plantStage)
    {
        if(_plantViews.TryGetValue(plantId, out var plantView))
        {
            plantView.SetPlantStage(plantStage);
        }
    }

    private void PlantDied(string plantId, bool alive)
    {
        if (_plantViews.TryGetValue(plantId, out var plantView))
        {
            plantView.SetPlantAlive(alive);
        }
    }

    public void ResetPlantWidgets()
    {
        foreach(var plantView in _plantViews.Values)
        {
            plantView.ResetButtonState();
        }
    }
}
