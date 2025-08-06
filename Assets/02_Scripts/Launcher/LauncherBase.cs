using UnityEngine;

namespace Launcher
{
    public class LauncherBase : MonoBehaviour
    {
        void Awake()
        {
            GlobalBase.SetLauncher(this);

            OnLauncherInitialized();
        }

        void Start()
        {
            OnLauncherStarted();
        }

        protected virtual void OnLauncherInitialized()
        {
            // Override this method in derived classes to handle initialization logic
        }

        protected virtual void OnLauncherStarted()
        {
            // Override this method in derived classes to handle startup logic
        }
    }
}