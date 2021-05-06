using System;

namespace GameWorkstore.Patterns
{
    [Serializable]
    public class SceneAssetPath
    {
        public string Scene;
        public string Guid;

        public void OnValidate()
        {
#if UNITY_EDITOR
            var oldScenePath = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(Scene);
            if (oldScenePath) return;

            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(Guid);
            if (string.IsNullOrEmpty(path)) return;
            Scene = path;
#endif
        }
    }
}
