using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using versoft.asset_manager;
using versoft.plant.game_logic;

public class PlantView : MonoBehaviour
{
    [SerializeField]
    private Animator plantAnimator;

    [SerializeField]
    private SpriteButton spriteButton;

    private string _plantId;

    public async void Initialize(PlantSavedData plantSavedData)
    {
        if (plantAnimator == null)
        { return; }

        var assetManager = ServiceLocator.Instance.Get<AssetManager>();
        if (assetManager == null)
        { return; }

        var path = Path.Combine(Const.AnimationFolder, plantSavedData.PlantId, plantSavedData.PlantId);
        var plantAnimatorOverride = await assetManager.LoadAsset<AnimatorOverrideController>(path, ".overrideController", false);
        if (plantAnimatorOverride == null)
        { return; }

        _plantId = plantSavedData.PlantInstanceId;

        plantAnimator.runtimeAnimatorController = plantAnimatorOverride;

        SetPlantStage(plantSavedData.PlantStage);
        SetPlantAlive(plantSavedData.Alive);
    }

    public void SetPlantStage(PlantStage plantStage)
    {
        plantAnimator.SetInteger("PlantStage", (int)plantStage);
    }

    public void SetPlantAlive(bool alive)
    {
        plantAnimator.SetBool("IsDead", !alive);
    }

    public void OnClick()
    {
        var gameManager = ServiceLocator.Instance.Get<GameManager>();
        if(gameManager == null) 
        { return; }

        gameManager.SelectPlant(_plantId);
        spriteButton.SetInteractable(false);
    }

    public void ResetButtonState()
    {
        spriteButton.SetInteractable(true);
    }
}
