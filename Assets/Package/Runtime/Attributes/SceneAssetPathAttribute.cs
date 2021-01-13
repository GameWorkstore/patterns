using UnityEngine;

namespace GameWorkstore.Patterns
{
    public class SceneAssetPathAttribute : PropertyAttribute
    {
    }
}

#if UNITY_EDITOR
namespace GameWorkstore.Patterns.Internal
{
    using UnityEditor;

    [CustomPropertyDrawer(typeof(SceneAssetPathAttribute))]
    public class SceneAssetPathAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var oldScenePath = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);

            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUI.ObjectField(position, "scene", oldScenePath, typeof(SceneAsset), false) as SceneAsset;
            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                property.stringValue = newPath;
            }
        }
    }
}
#endif
