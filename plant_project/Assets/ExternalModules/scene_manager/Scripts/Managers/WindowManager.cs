using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using versoft.asset_manager;

namespace versoft.scene_manager
{
    public class WindowHistory
    {
        public string ActiveScene;
        public WindowController WindowController;
        public UILayers Layer;
        public bool ShowDimmer;
    }

    public class WindowManager : MonoBehaviour
    {
        [SerializeField]
        private LayerController layerController;

        public Canvas MainCanvas { get; private set; }
        private Stack<WindowHistory> windowStack = new Stack<WindowHistory>();
        private Dictionary<string, LoadingScreenWindowController> _loadingScreens = new Dictionary<string, LoadingScreenWindowController>();

        private AssetManager assetManager;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            assetManager = ServiceLocator.Instance.Get<AssetManager>();
            MainCanvas = GetComponentInChildren<Canvas>();
            GetLoadingScreens();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (windowStack.TryPeek(out WindowHistory history))
                {
                    history.WindowController.OnBackButton();
                }
            }
        }

        private async void GetLoadingScreens()
        {
            System.Type type = typeof(LoadingScreenType);
            FieldInfo[] properties = type.GetFields();
            foreach (var property in properties)
            {
                string loadingType = (string)property.GetValue(null);
                if (loadingType != LoadingScreenType.None)
                {
                    var loadingScreen = await assetManager.LoadAsset<LoadingScreenWindowController>(Const.LoadingScreenFolder + loadingType, instantiate: true);
                    if (loadingScreen != null)
                    {
                        layerController.AddToLayer(UILayers.Loading, loadingScreen, false);
                        loadingScreen.gameObject.SetActive(false);
                        _loadingScreens.Add(loadingType, loadingScreen);
                    }
                }
            }
        }


        public async void OpenScreen(string windowName, UILayers layer, bool hidePreviousWindow = true, bool showDimmer = false, WindowConfig config = null, string loadingScreenType = LoadingScreenType.None, string sceneName = "", bool clearHistory = false)
        {
            string addressablePath = Const.ScreensFolder + windowName;
            var windowController = await assetManager.LoadAsset<WindowController>(addressablePath);
            if (windowController != null)
            {
                // Show Loading screen.
                await ShowLoading(loadingScreenType);

                // If we have to load a scene, we do it here
                if (!string.IsNullOrEmpty(sceneName))
                {
                    await LoadScene(sceneName);
                }

                // Instantiate the new window
                var windowInstance = Instantiate<WindowController>(windowController);

                // Add it to the layer controller so it shows properly.
                layerController.AddToLayer(layer, windowInstance, showDimmer);

                // Here we Initialize the config.
                await windowInstance.Init(config);

                // Disable the previous window.
                if (clearHistory)
                {
                    ClearHistory();
                }
                else if (hidePreviousWindow && windowStack.TryPeek(out var previousWindow) && previousWindow.WindowController)
                {
                    if (previousWindow.WindowController != null)
                    {
                        await previousWindow.WindowController.OnWindowWillDisable();
                        previousWindow.WindowController.gameObject.SetActive(false);
                    }
                }

                string historySceneName = sceneName;
                if (string.IsNullOrEmpty(sceneName))
                {
                    historySceneName = SceneManager.GetActiveScene().name;
                }

                // Add it to the stack.
                WindowHistory history = new WindowHistory
                {
                    WindowController = windowInstance,
                    Layer = layer,
                    ShowDimmer = showDimmer,
                    ActiveScene = historySceneName
                };
                windowStack.Push(history);

                // Hide the Loading
                await HideLoading();

                // Start the Transition
                await windowInstance.TransitionIn();
            }
            else
            {
                Debug.LogError($"Noooope. We can't find {windowName}");
            }
        }

        //
        private void ClearHistory()
        {
            foreach (var history in windowStack)
            {
                layerController.RemoveFromLayer(history.Layer, history.WindowController);
            }
            windowStack.Clear();
        }

        private string FormatScenePath(string sceneName)
        {
            return asset_manager.Const.BaseAddressablesAddress + Const.ScenesFolder + sceneName + asset_manager.Const.SceneExtension;
        }

        private async Task LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            { return; }

            var scenePath = FormatScenePath(sceneName);

            var locationOperation = Addressables.LoadResourceLocationsAsync(scenePath);
            while (!locationOperation.IsDone)
            {
                await Task.Delay(100);
            }

            if (locationOperation.Result != null && locationOperation.Result.Count > 0)
            {
                var loadOperation = Addressables.LoadSceneAsync(scenePath);
                while (!loadOperation.IsDone)
                {
                    await Task.Delay(100);
                }

                return;
            }

            var index = SceneUtility.GetBuildIndexByScenePath(sceneName);
            if (index < 0)
            { return; }


            var normalLoadOperation = SceneManager.LoadSceneAsync(index);
            while (!normalLoadOperation.isDone)
            {
                await Task.Delay(100);
            }
        }

        public async void CloseScreen(WindowController windowController)
        {
            if (!windowStack.TryPeek(out var history))
            {
                Debug.LogError("Stack is empty");
                return;
            }

            if (windowStack.Count < 2)
            {
                Debug.Log("There's only one Screen open... We can't close that");
                return;
            }

            if (history.WindowController != windowController)
            {
                Debug.LogError("You are trying to close a window that's not on top [Not Supported yet]");
                return;
            }

            // Get the current Screen out
            windowStack.Pop();

            // Transition out
            await history.WindowController.TransitionOut();

            // Disable the previous window.
            history.WindowController.gameObject.SetActive(false);

            // The previous window will send the last callback
            history.WindowController.OnWindowClosed();

            // Remove the previous from layer;
            layerController.RemoveFromLayer(history.Layer, history.WindowController);

            // Reenable the previous window and add it to the layer.
            var previousHistory = windowStack.Peek();

            bool changeScene = previousHistory.ActiveScene != SceneManager.GetActiveScene().name;
            if (changeScene)
            {
                await ShowLoading(LoadingScreenType.Spinner);
                await LoadScene(previousHistory.ActiveScene);
            }

            previousHistory.WindowController.gameObject.SetActive(true);
            layerController.AddToLayer(previousHistory.Layer, previousHistory.WindowController, previousHistory.ShowDimmer);

            if (changeScene)
            {
                await HideLoading();
            }
        }

        private async Task HideLoading()
        {
            foreach (var loadingScreen in _loadingScreens.Values)
            {
                if (loadingScreen.gameObject.activeInHierarchy)
                {
                    await loadingScreen.FadeOut();
                    loadingScreen.gameObject.SetActive(false);
                }
            }
        }

        private async Task ShowLoading(string screenType)
        {
            if (screenType == LoadingScreenType.None)
            { return; }

            if (_loadingScreens.TryGetValue(screenType, out var loadingScreen))
            {
                loadingScreen.gameObject.SetActive(true);
                await loadingScreen.FadeIn();
            }
        }
    }
}