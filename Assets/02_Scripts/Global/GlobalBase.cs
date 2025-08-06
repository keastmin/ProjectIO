using UnityEngine;

public class GlobalBase : MonoBehaviour
{
    public static Launcher.LauncherBase Launcher { get; private set; }

    public static void SetLauncher(Launcher.LauncherBase launcher)
    {
        Launcher = launcher;
    }
}