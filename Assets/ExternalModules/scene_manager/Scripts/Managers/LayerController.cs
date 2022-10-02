using System.Collections.Generic;
using UnityEngine;

namespace versoft.scene_manager
{
    public enum UILayers
    {
        Screen,
        Popup,
        Critical,
        Loading
    }

    [System.Serializable]
    public class LayerHolder
    {
        public UILayers Layer;
        public GameObject LayerContainer;
        public GameObject Dimmer;
    }

    public class LayerController : MonoBehaviour
    {
        [SerializeField]
        private List<LayerHolder> layers;

        private Dictionary<UILayers, LayerHolder> _accessibleLayers = new Dictionary<UILayers, LayerHolder>();


        private void Awake()
        {
            foreach (var layerHolder in layers)
            {
                _accessibleLayers.Add(layerHolder.Layer, layerHolder);
            }
        }

        public void AddToLayer(UILayers layer, WindowController windowController, bool showDimmer)
        {
            if (_accessibleLayers.TryGetValue(layer, out var layerHolder))
            {
                if (layerHolder.Dimmer != null)
                {
                    layerHolder.Dimmer.SetActive(showDimmer);
                    layerHolder.Dimmer.transform.SetAsLastSibling();
                }

                windowController.transform.SetParent(layerHolder.LayerContainer.transform);
                windowController.transform.SetAsLastSibling();
                var rectTransform = windowController.transform as RectTransform;
                if (rectTransform != null)
                {
                    rectTransform.ResetRectTransform();
                }
            }
            else
            {
                Debug.LogError($"We don't have a layer holder for {layer}");
            }
        }

        public void RemoveFromLayer(UILayers layer, WindowController windowController)
        {
            if (_accessibleLayers.TryGetValue(layer, out var layerHolder))
            {
                Destroy(windowController.gameObject);
                if (layerHolder.Dimmer != null)
                {
                    layerHolder.Dimmer.SetActive(false);
                }
            }
            else
            {
                Debug.LogError($"We don't have a layer holder for {layer}");
            }
        }
    }
}