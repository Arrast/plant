using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using versoft.asset_manager;
using versoft.plant.game_logic;

public class PlantLayoutElement : MonoBehaviour
{
    [SerializeField]
    private List<Transform> plantPositions;

    [SerializeField]
    private SpriteRenderer plantHolderBackground;

    [SerializeField]
    private string plantLayoutElementId;

    private Dictionary<string, PlantView> _plantViews = new Dictionary<string, PlantView>();
    public Dictionary<string, PlantView> Plantviews => _plantViews;

    public void Initialize(PlantLayoutElementSaveData layoutSaveData)
    {
        if (!layoutSaveData.Unlocked)
        {
            plantHolderBackground.SafeSetActive(false);
            gameObject.SetActive(false);
            return;
        }

        LoadIcon(layoutSaveData.Skin);
    }

    public void SetPlant(string plantId, PlantView plantInstance, int index)
    {
        plantInstance.transform.SetParent(plantPositions[index]);
        plantInstance.transform.localPosition = Vector3.zero;
        plantInstance.transform.localRotation = Quaternion.identity;
        plantInstance.transform.localScale = Vector3.one;
        _plantViews.Add(plantId, plantInstance);
    }

    private async void LoadIcon(string iconId)
    {
        if (string.IsNullOrEmpty(iconId) || plantHolderBackground == null)
        { return; }

        var assetManager = ServiceLocator.Instance.Get<AssetManager>();
        if (assetManager == null)
        { return; }

        string iconPath = $"Sprites/Game Assets/Shelves/{iconId}";
        var sprite = await assetManager.LoadAsset<Sprite>(iconPath);
        if(sprite == null) 
        { return; }

        plantHolderBackground.SafeSetActive(true);
        plantHolderBackground.sprite = sprite;
    }
}
