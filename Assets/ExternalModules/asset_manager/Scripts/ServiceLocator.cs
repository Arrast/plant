using System.Collections.Generic;
using UnityEngine;
using versoft.singleton;

namespace versoft.asset_manager
{
    public class ServiceLocator : Singleton<ServiceLocator>
    {
        private Dictionary<System.Type, object> _services = new Dictionary<System.Type, object>();

        public void RegisterMonoBehaviour<T>() where T : MonoBehaviour
        {
            if (!_services.ContainsKey(typeof(T)))
            {
                var type = typeof(T);
                GameObject holder = new GameObject(type.ToString());
                DontDestroyOnLoad(holder);
                T service = holder.AddComponent<T>();
                _services.Add(type, service);
            }
        }

        public void Register<T>()
        {
            T service = System.Activator.CreateInstance<T>();
            Register<T>(service);
        }

        public void Register<T>(T service) 
        {
            if (!_services.ContainsKey(typeof(T)))
            {
                _services.Add(typeof(T), service);
            }
        }

        public T Get<T>()
        {
            if (_services.TryGetValue(typeof(T), out var rawService) && rawService is T castedService)
            {
                return castedService;
            }
            return default;
        }
    }
}