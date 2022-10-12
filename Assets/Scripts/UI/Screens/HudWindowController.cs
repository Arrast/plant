using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using versoft.asset_manager;
using versoft.plant.game_logic;
using versoft.scene_manager;

public class HudWindowController : WindowController
{
    private PlantManager _plantManager;

    [SerializeField]
    private List<ResourceUIWidget> plantResourceWidgets;

    [SerializeField]
    private GameObject plantResourceContainer;

    [SerializeField]
    private GameObject deselectPlantButton;

    private Dictionary<PlantStat, ResourceUIWidget> _resourceWidgets = new Dictionary<PlantStat, ResourceUIWidget>();

    protected override void Awake()
    {
        base.Awake();
        if (plantResourceWidgets == null)
        { return; }

        foreach (var resourceWidget in plantResourceWidgets)
        {
            _resourceWidgets.Add(resourceWidget.PlantResource, resourceWidget);
        }
    }

    public override Task Init(WindowConfig windowConfig)
    {
        _plantManager = ServiceLocator.Instance.Get<PlantManager>();
        if (_plantManager != null)
        {
            _plantManager.OnTick += UpdatePlantStats;
            _plantManager.OnPlantSelected += PlantSelected;
        }
        return base.Init(windowConfig);
    }

    private void PlantSelected(PlantLogic selectedPlant)
    {
        UpdatePlantStats();
    }

    public void DeselectPlant()
    {
        var gameManager = ServiceLocator.Instance.Get<GameManager>();
        gameManager.DeselectPlant();
    }

    private void UpdatePlantStats()
    {
        PlantLogic selectedPlant = _plantManager.SelectedPlant;
        plantResourceContainer.SafeSetActive(selectedPlant != null);
        deselectPlantButton.SafeSetActive(selectedPlant != null);

        if (selectedPlant == null)
        {
            return;
        }

        foreach(var pair in _resourceWidgets)
        {
            float resourceValue = selectedPlant.GetStatValue(pair.Key);
            pair.Value.SetResourceValue(resourceValue);
        }
    }
}
