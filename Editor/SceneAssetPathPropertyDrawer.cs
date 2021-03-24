using UnityEngine;
using UnityEditor;

namespace GameWorkstore.Patterns.Editor
{
    [CustomPropertyDrawer(typeof(SceneAssetPath))]
    public class SceneAssetPathPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propertyScene = property.FindPropertyRelative("Scene");
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(propertyScene.stringValue);
            return oldScene ? 20 : 60;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            var propertyScene = property.FindPropertyRelative("Scene");
            var propertyGuid = property.FindPropertyRelative("Guid");

            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(propertyScene.stringValue);
            if (!oldScene)
            {
                var path = AssetDatabase.GUIDToAssetPath(propertyGuid.stringValue);
                if (!string.IsNullOrEmpty(path))
                {
                    oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                }
            }

            EditorGUI.BeginChangeCheck();

            var rt1 = position;
            rt1.size = new Vector2(position.size.x, 20);
            var newScene = EditorGUI.ObjectField(rt1, "Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;
            if (!newScene)
            {
                position.position += new Vector2(0, rt1.size.y);
                var rt2 = position;
                rt2.size = new Vector2(position.size.x, 40);
                EditorGUI.HelpBox(rt2, "SceneAssets are weak references. To ensure consistency when moving scenes, declare OnValidate() function on MonoBehaviour parent and call the OnValidate() of this variable to ensure consistenty by GUID.", MessageType.Info);
            }
            if (!EditorGUI.EndChangeCheck()) return;
            var newPath = AssetDatabase.GetAssetPath(newScene);
            propertyScene.stringValue = newPath;
            propertyGuid.stringValue = AssetDatabase.AssetPathToGUID(newPath);
        }
    }
}
