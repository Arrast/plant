using UnityEngine;
using UnityEngine.UI;
using versoft.asset_manager;
using versoft.plant.game_logic;

public class ResourceUIWidget : MonoBehaviour
{
    [SerializeField]
    private PlantStat plantResource;

    [SerializeField]
    private Image resourceIcon;

    [SerializeField]
    private Image resourceProgressBar;

    public PlantStat PlantResource => plantResource;

    public async void Start()
    {
        SetResourceValue(0);
        var assetManager = ServiceLocator.Instance.Get<AssetManager>();
        if(assetManager == null) 
        { return; }

        string resourcePath = $"Sprites/UI/resource_{plantResource.ToString().ToLower()}";
        var sprite = await assetManager.LoadAsset<Sprite>(resourcePath);
        if(sprite != null)
        {
            resourceIcon.sprite = sprite;
        }
    }

    public void SetResourceValue(float value)
    {
        resourceProgressBar.fillAmount = value;

    }
}
