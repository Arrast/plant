using UnityEngine;
using UnityEngine.UI;
using versoft.asset_manager;
using versoft.plant.game_logic;

public class ResourceIcon : MonoBehaviour
{
    [SerializeField]
    private Image resourceIcon;

    public async void Init(PlantStat plantResource)
    {
        var assetManager = ServiceLocator.Instance.Get<AssetManager>();
        if (assetManager == null)
        { return; }

        string resourcePath = $"Sprites/UI/resource_{plantResource.ToString().ToLower()}";
        var sprite = await assetManager.LoadAsset<Sprite>(resourcePath);
        if (sprite != null)
        {
            resourceIcon.sprite = sprite;
        }
    }
}
