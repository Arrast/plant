using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace versoft.asset_manager
{
    public class AssetManager
    {
        // TODO (victor): Do we need to implement a lifetime?
        private Dictionary<string, Object> _cache = new Dictionary<string, Object>();

        public async Task<T> LoadAsset<T>(string path, string extension = "", bool instantiate = false) where T : Object
        {
            if (string.IsNullOrEmpty(extension))
            {
                extension = GetDefaultExtension<T>();
            }

            System.Type type = typeof(T);
            T assetReference = null;

            if (type.IsSubclassOf(typeof(Component)))
            {
                assetReference = await GetAssetAndGetComponent<T>(path, extension);
            }
            else
            {
                assetReference = await GetAsset<T>(path, extension);
            }


            if (assetReference == null)
            {
                LogFailure(path + extension);
                return null;
            }

            if (instantiate)
            {
                T assetInstance = GameObject.Instantiate<T>(assetReference);
                return assetInstance;
            }
            return assetReference;
        }

        public async void PreloadAsset(string path, string extension)
        {
            var assetReference = await GetAsset<Object>(path, extension);
            if (assetReference == null)
            {
                LogFailure(path + extension);
            }
        }

        private async Task<T> GetAsset<T>(string path, string extension) where T : Object
        {
            Object cachedElement;
            if (_cache.TryGetValue(path, out cachedElement))
            {
                return cachedElement as T;
            }

            try
            {
                var fullPath = Path.Combine(Const.BaseAddressablesAddress, path + extension);
                var assetLoadingHandle = Addressables.LoadAssetAsync<T>(fullPath);
                await assetLoadingHandle.Task;
                if (assetLoadingHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    if (_cache.TryGetValue(path, out cachedElement))
                    {
                        return cachedElement as T;
                    }
                    _cache.Add(path, assetLoadingHandle.Result);
                    return assetLoadingHandle.Result as T;
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
                return null;
            }
        }

        private async Task<T> GetAssetAndGetComponent<T>(string path, string extension) where T : Object
        {
            var assetReference = await GetAsset<GameObject>(path, extension);
            if (assetReference == null)
            {
                UnityEngine.Debug.LogError($"We can't find the  asset {path + extension}");
                return null;
            }

            var componentReference = assetReference.GetComponent<T>();
            if (componentReference == null)
            {
                UnityEngine.Debug.LogError($"The asset {path + extension} doesn't have a component of type {typeof(T)}");
                return null;
            }
            return componentReference;
        }

        // This method will grow if we need to find elements of other types.
        private string GetDefaultExtension<T>()
        {
            System.Type type = typeof(T);
            if (type.IsAssignableFrom(typeof(Animation)))
            {
                return Const.AnimationExtension;
            }

            if (type.IsAssignableFrom(typeof(Scene)))
            {
                return Const.SceneExtension;
            }

            if (type.IsAssignableFrom(typeof(Texture)) || type.IsAssignableFrom(typeof(Sprite)))
            {
                return Const.ImageExtension;
            }
            return Const.PrefabExtension;
        }

        private void LogFailure(string assetPath)
        {
            UnityEngine.Debug.LogError($"We can't find {assetPath}");
        }
    }
}