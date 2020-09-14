using UnityEditor;
using UnityEditor.PackageManager;

public class PackageUpdater
{
    [MenuItem("Help/PackageUpdate/Patterns")]
    public static void TrackPackages()
    {
        Client.Add("git://github.com/GameWorkstore/patterns.git");
    }
}
