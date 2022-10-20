using System;
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

    [SerializeField]
    private FakeShadow shadowComponent;

    [SerializeField]
    private SpriteRenderer sunScreen;

    private string _plantId;
    private GameTimeManager _gameTimeManager;

    public async void Initialize(PlantSavedData plantSavedData)
    {
        if (plantAnimator == null)
        { return; }

        _gameTimeManager = ServiceLocator.Instance.Get<GameTimeManager>();
        if (_gameTimeManager == null)
        { return; }

        _gameTimeManager.SetOnTimeOfDayChangedListener(UpdateShadow);

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
        UpdateShadow(_gameTimeManager.GetTimeOfDayFromTime());
        SetSunScreen(false);
    }

    private void UpdateShadow(TimeOfDayEnum timeOfDay)
    {
        SetShadow(timeOfDay == TimeOfDayEnum.Night || timeOfDay == TimeOfDayEnum.Dawn);
    }

    public void SetShadow(bool enabled)
    {
        if (shadowComponent != null)
        {
            shadowComponent.ToggleShadow(enabled);
        }
    }

    private void SetSunScreen(bool enabled)
    {
        sunScreen.SafeSetActive(enabled);
        if (!enabled)
        {
            UpdateShadow(_gameTimeManager.GetTimeOfDayFromTime());
        }
        else
        {
            SetShadow(true);
        }
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
        if (gameManager == null)
        { return; }

        gameManager.SelectPlant(_plantId);
        spriteButton.SetInteractable(false);
    }

    public void ResetButtonState()
    {
        spriteButton.SetInteractable(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) { SetSunScreen(true); }
        if (Input.GetKeyDown(KeyCode.S)) { SetSunScreen(false); }
    }
}
