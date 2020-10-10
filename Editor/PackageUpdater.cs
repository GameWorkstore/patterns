#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;

namespace GameWorkstore.Patterns
{
    public class PackageUpdater
    {
        [MenuItem("Help/PackageUpdate/GameWorkstore.Patterns")]
        public static void TrackPackages()
        {
            Client.Add("git://github.com/GameWorkstore/patterns.git");
        }
    }
}
#endif