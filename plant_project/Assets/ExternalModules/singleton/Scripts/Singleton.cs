using UnityEngine;
namespace versoft.singleton
{
    public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    CreateInstance();
                }

                return _instance;
            }
        }

        private static void CreateInstance()
        {
            GameObject gameObjectInstance = new GameObject(typeof(T) + "_Instance");
            GameObject.DontDestroyOnLoad(gameObjectInstance);

            _instance = gameObjectInstance.AddComponent<T>();
        }
    }
}
