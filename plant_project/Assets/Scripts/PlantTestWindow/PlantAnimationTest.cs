using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using versoft.asset_manager;
using versoft.plant.game_logic;

public class PlantAnimationTest : MonoBehaviour
{
    [SerializeField]
    private float timeToLoop;

    [SerializeField]
    private Animator plantAnimator;

    [SerializeField]
    private string plantId;

    private PlantStage PlantStage = PlantStage.Seedling;

    public async void StartTest()
    {
        if (string.IsNullOrEmpty(plantId))
        {
            Destroy(gameObject);
            return;
        }

        var assetManager = ServiceLocator.Instance.Get<AssetManager>();
        if(assetManager == null)
        { return; }

        if (plantAnimator == null)
        { return; }


        var path = Path.Combine(Const.AnimationFolder, plantId, plantId);
        var plantAnimatorOverride = await assetManager.LoadAsset<AnimatorOverrideController>(path, ".overrideController", false);
        if (plantAnimatorOverride == null)
        {
            Destroy(gameObject);
            return;
        }

        plantAnimator.runtimeAnimatorController = plantAnimatorOverride;

        StartCoroutine(ChangeState());
    }

    private IEnumerator ChangeState()
    {
        if (timeToLoop <= 0)
        {
            yield break;
        }

        while (true)
        {
            yield return new WaitForSeconds(timeToLoop);
            var intStage = PlantStage + 1;
            if(intStage > PlantStage.Grown)
            {
                SetPlantAlive(false);
                yield return new WaitForSeconds(timeToLoop);
                SetPlantAlive(true);
                plantAnimator.SetTrigger("Reset");
                PlantStage = 0;
                SetPlantStage(PlantStage);
            }
            else
            {
                PlantStage = intStage;
                SetPlantStage(PlantStage);
            }
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
}
