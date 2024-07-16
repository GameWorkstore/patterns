using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameWorkstore.Patterns
{
    public enum HelpBoxType
    {
        None,
        Info,
        Warning,
        Error
    }

    [Serializable]
    public class HelpBox
    {
        [NonSerialized] public string Text;
        [NonSerialized] public float Height;
        [NonSerialized] public HelpBoxType Type;

        public HelpBox(string text, float height, HelpBoxType type = HelpBoxType.Info)
        {
            Text = text;
            Height = height;
            Type = type;
        }

        public HelpBox(string text, HelpBoxType type = HelpBoxType.Info)
        {
            Text = text;
            Height = 40;
            Type = type;
        }

        public HelpBox()
        {
            Height = 40;
            Text = "";
            Type = HelpBoxType.Info;
        }
    }
}

#if UNITY_EDITOR
namespace GameWorkstore.Patterns.Internal
{
    [CustomPropertyDrawer(typeof(HelpBox))]
    public class HelpBoxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var info = fieldInfo.GetValue(property.serializedObject.targetObject) as HelpBox;

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.HelpBox(position, info.Text, (MessageType)info.Type);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var info = fieldInfo.GetValue(property.serializedObject.targetObject) as HelpBox;
            return info.Height;
        }
    }
}
#endif