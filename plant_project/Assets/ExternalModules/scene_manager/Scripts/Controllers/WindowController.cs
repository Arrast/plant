using System;
using System.Threading.Tasks;
using UnityEngine;
using versoft.asset_manager;

namespace versoft.scene_manager
{
    public class WindowConfig
    {
        public object Payload = null;
        public string Scene = string.Empty;
    }

    public class WindowController : MonoBehaviour
    {
        private WindowManager _windowManager;
        protected WindowManager WindowManager
        {
            get
            {
                if(_windowManager == null)
                {
                    _windowManager = ServiceLocator.Instance.Get<WindowManager>();
                }
                return _windowManager;
            }
        }

        /// <summary>
        /// The scene model.
        /// </summary>
        public WindowConfig WindowConfig;

        protected virtual void Awake()
        {

        }

        public virtual Task Init(WindowConfig windowConfig)
        {
            // We set the window config.
            WindowConfig = windowConfig;

            // We just return null for now so it doesn't complain..
            return Task.CompletedTask;
        }

        public virtual void OnBackButton()
        {
            Close();
        }

        public virtual void Close()
        {
            var windowManager = ServiceLocator.Instance.Get<WindowManager>();
            if (windowManager != null)
            {
                windowManager.CloseScreen(this);
            }
        }

        public virtual Task OnWindowWillDisable()
        {
            return Task.CompletedTask;
        }

        public virtual Task TransitionIn()
        {
            return Task.CompletedTask;
        }

        public virtual Task TransitionOut()
        {
            return Task.CompletedTask;
        }

        public virtual void OnWindowClosed()
        {

        }

        public virtual void WindowOpened()
        {

        }
    }
}