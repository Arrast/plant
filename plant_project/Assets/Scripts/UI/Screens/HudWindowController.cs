using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using versoft.asset_manager;
using versoft.plant.game_logic;
using versoft.scene_manager;

public class HudWindowController : WindowController
{
    private GameManager _gameManager;
    private PlantManager _plantManager;

    [SerializeField]
    private List<ResourceUIWidget> plantResourceWidgets;

    [SerializeField]
    private List<ResourceUpdateWidget> plantIncreaseResourceWidgets;

    [SerializeField]
    private List<ResourceToggleWidget> plantResourceToggleWidgets;

    [SerializeField]
    private GameObject selectedPlantOverlay;

    [SerializeField]
    private GameObject defaultOverlay;

    [Header("Top Bar")]
    [SerializeField] 
    private CurrencyWidget softCurrencyWidget;

    private Dictionary<PlantStat, ResourceUIWidget> _resourceWidgets = new Dictionary<PlantStat, ResourceUIWidget>();

    protected override void Awake()
    {
        base.Awake();
        if (plantResourceWidgets != null)
        {
            foreach (var resourceWidget in plantResourceWidgets)
            {
                _resourceWidgets.Add(resourceWidget.PlantResource, resourceWidget);
            }
        }

        if (plantIncreaseResourceWidgets != null)
        {
            foreach (var plantIncreaseWidget in plantIncreaseResourceWidgets)
            {
                plantIncreaseWidget.Init(StartIncreaseResource, StopIncreasingResource);
            }
        }

        if (plantResourceToggleWidgets != null)
        {
            foreach (var widget in plantResourceToggleWidgets)
            {
                widget.Init(false, OnResourceToggleChanged);
            }
        }
    }

    private void UpdateSoftCurrency(int softCurrencyAmount)
    {
        if(softCurrencyWidget == null)
        { return; }

        softCurrencyWidget.UpdateAmount(softCurrencyAmount);
    }

    private void OnResourceToggleChanged(PlantStat plantStat, bool isOn)
    {
        if (_plantManager == null || _gameManager == null)
        { return; }

        PlantLogic selectedPlant = _plantManager.SelectedPlant;
        if (selectedPlant == null)
        { return; }

        _plantManager.ToggleResourceModifier(plantStat, isOn);
        _gameManager.ToggleResourceOnPlantView(selectedPlant.GetPlantSaveData().PlantInstanceId, plantStat, isOn);
    }

    private void StartIncreaseResource(PlantStat plantStat)
    {
        if (_plantManager != null)
        {
            _plantManager.SetResourceState(plantStat, true);
        }
    }

    private void StopIncreasingResource(PlantStat plantStat)
    {
        if (_plantManager != null)
        {
            _plantManager.SetResourceState(plantStat, false);
        }
    }

    public override async Task Init(WindowConfig windowConfig)
    {
        _gameManager = ServiceLocator.Instance.Get<GameManager>();
        _plantManager = ServiceLocator.Instance.Get<PlantManager>();
        if (_plantManager != null)
        {
            _plantManager.OnTick += UpdatePlantStats;
            _plantManager.OnPlantSelected += PlantSelected;
        }

        var playerManager = ServiceLocator.Instance.Get<PlayerManager>();
        if (playerManager != null)
        {

            if (softCurrencyWidget == null)
            { return; }

            softCurrencyWidget.SetCurrency(Const.SoftCurrencyId, playerManager.GetSoftCurrency());
            playerManager.AddCurrencyModifiedListener(UpdateSoftCurrency);
        }

        await base.Init(windowConfig);
    }

    private void PlantSelected(PlantLogic selectedPlant)
    {
        if (plantResourceToggleWidgets != null)
        {
            foreach (var widget in plantResourceToggleWidgets)
            {
                bool hasStat = (selectedPlant == null) ? false : selectedPlant.HasModifierForStat(widget.PlantStat);
                widget.SetState(hasStat);
            }
        }
        UpdatePlantStats();
    }

    public void DeselectPlant()
    {
        _gameManager.DeselectPlant();
        if (plantResourceToggleWidgets != null)
        {
            foreach (var widget in plantResourceToggleWidgets)
            {
                widget.SetState(false);
            }
        }
    }

    private void UpdatePlantStats()
    {
        PlantLogic selectedPlant = _plantManager.SelectedPlant;
        selectedPlantOverlay.SafeSetActive(selectedPlant != null);
        defaultOverlay.SafeSetActive(selectedPlant == null);

        if (selectedPlant == null)
        {
            return;
        }

        foreach (var pair in _resourceWidgets)
        {
            float resourceValue = selectedPlant.GetStatValue(pair.Key);
            pair.Value.SetResourceValue(resourceValue);
        }
    }

    public void OpenShop()
    {
        var windowManager = ServiceLocator.Instance.Get<WindowManager>();
        if(windowManager == null)
        {
            return;
        }

        windowManager.OpenScreen("2.0_Shop", UILayers.Screen, sceneName: "ShopScene");
    }
}
