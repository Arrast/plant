using UnityEngine;
using versoft.asset_manager;
using versoft.plant.game_logic;

public class ResourceToggleWidget : MonoBehaviour
{
    private const string uiSpritesFolder = "Sprites/UI/";

    public PlantStat PlantStat;

    [SerializeField]
    private ToggleState toggleComponent;

    private System.Action<PlantStat, bool> _onStateChanged;

    public async void Init(bool isOn, System.Action<PlantStat, bool> onStateChanged)
    {
        _onStateChanged = onStateChanged;

        // Let's load the icons 
        var assetManager = ServiceLocator.Instance.Get<AssetManager>();
        if (assetManager == null)
        { return; }

        string baseSpritePath = uiSpritesFolder + $"resource_button_{PlantStat.ToString().ToLower()}";
        var onSprite = await assetManager.LoadAsset<Sprite>(baseSpritePath + "_on");
        var offSprite = await assetManager.LoadAsset<Sprite>(baseSpritePath + "_off");

        if(onSprite == null || offSprite == null) 
        { return; }

        toggleComponent.OnSprite = onSprite;
        toggleComponent.OffSprite = offSprite;
        toggleComponent.Init(isOn);
    }

    public void SetState(bool isOn)
    {
        toggleComponent.ForceToggleState(isOn);
    }

    public void OnResourceChanged(bool isOn)
    {
        _onStateChanged?.Invoke(PlantStat, isOn);
    }
}
