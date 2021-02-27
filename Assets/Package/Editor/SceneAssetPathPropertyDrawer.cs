using UnityEngine;
using UnityEditor;

namespace GameWorkstore.Patterns
{
    [CustomPropertyDrawer(typeof(SceneAssetPath))]
    public class SceneAssetPathPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 60;
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
            position.position += new Vector2(0, rt1.size.y);
            var rt2 = position;
            rt2.size = new Vector2(position.size.x, 40);

            var newScene = EditorGUI.ObjectField(rt1, "Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;
            EditorGUI.HelpBox(rt2, "Scene assets can de-sync if scenes are moved in few cases, as their reference is weak. Reimport asset can fix the missing references automatically sometimes.", MessageType.Info);
            if (!EditorGUI.EndChangeCheck()) return;
            var newPath = AssetDatabase.GetAssetPath(newScene);
            propertyScene.stringValue = newPath;
            propertyGuid.stringValue = AssetDatabase.AssetPathToGUID(newPath);
        }
    }
}
