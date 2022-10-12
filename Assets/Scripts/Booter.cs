using UnityEngine;
using versoft.asset_manager;
using versoft.data_model;
using versoft.save_data;
using versoft.scene_manager;

public class Booter : MonoBehaviour
{
    // Start is called before the first frame update
    async void Awake()
    {
        var assetManager = new AssetManager();
        ServiceLocator.Instance.Register<AssetManager>(assetManager);

        var windowManager = await assetManager.LoadAsset<WindowManager>("Prefabs/MainUI", instantiate: true);
        if (windowManager != null)
        {
            ServiceLocator.Instance.Register<WindowManager>(windowManager);
        }

        ServiceLocator.Instance.Register<DataModelDatabase>();
        ServiceLocator.Instance.Register<SaveDataManager>();
        ServiceLocator.Instance.RegisterMonoBehaviour<PlantManager>();
        ServiceLocator.Instance.Register<PlayerManager>();

        // Add the game manager... May not be here
        GameObject gameManagerGameObject = new GameObject("Game Manager");
        DontDestroyOnLoad(gameManagerGameObject);
        var gameManager = gameManagerGameObject.AddComponent<GameManager>();
        gameManager.Init();

        ServiceLocator.Instance.Register(gameManager);

        // Open the Hud
        windowManager.OpenScreen("1.0_HUD", UILayers.Screen);
    }

}
