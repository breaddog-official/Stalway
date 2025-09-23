#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Breaddog.Extensions
{
    public class DrawShapeAttribute : PropertyAttribute
    {
        public int maxWidth;
        public int maxHeight;

        public DrawShapeAttribute(int maxWidth = 8, int maxHeight = 8)
        {
            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;
        }
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DrawShapeAttribute))]
    public class DrawShapeAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int height = property.FindPropertyRelative("height").intValue;
            return EditorGUIUtility.singleLineHeight * (2 + height);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as DrawShapeAttribute;

            SerializedProperty widthProp = property.FindPropertyRelative("width");
            SerializedProperty heightProp = property.FindPropertyRelative("height");
            SerializedProperty dataProp = property.FindPropertyRelative("data");

            // Заголовок
            Rect labelRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);
            float yOffset = labelRect.yMax + 2;

            EditorGUI.BeginChangeCheck();

            // Поля W и H
            float labelWidth = 40;
            float fieldWidth = 40;
            float spacing = 10;

            Rect wLabelRect = new(position.x, yOffset, labelWidth, EditorGUIUtility.singleLineHeight);
            Rect wFieldRect = new(wLabelRect.xMax + 2, yOffset, fieldWidth, EditorGUIUtility.singleLineHeight);

            Rect hLabelRect = new(wLabelRect.xMax + spacing, yOffset, labelWidth, EditorGUIUtility.singleLineHeight);
            Rect hFieldRect = new(hLabelRect.xMax + 2, yOffset, fieldWidth, EditorGUIUtility.singleLineHeight);

            //EditorGUI.LabelField(wLabelRect, "W");
            widthProp.intValue = Mathf.Clamp(EditorGUI.IntField(wLabelRect, GUIContent.none, widthProp.intValue), 1, attr.maxWidth);

            //EditorGUI.LabelField(hLabelRect, "H");
            heightProp.intValue = Mathf.Clamp(EditorGUI.IntField(hLabelRect, GUIContent.none, heightProp.intValue), 1, attr.maxHeight);

            yOffset += EditorGUIUtility.singleLineHeight + 4;

            int width = widthProp.intValue;
            int height = heightProp.intValue;
            int requiredSize = width * height;

            // Обновление массива данных при необходимости
            if (dataProp.arraySize != requiredSize)
            {
                dataProp.arraySize = requiredSize;
                property.serializedObject.ApplyModifiedProperties(); // фикс размеров
                return;
            }

            // Рисуем сетку
            float toggleSize = 20f;
            for (int y = 0; y < height; y++)
            {
                Rect rowRect = new(position.x, yOffset + y * toggleSize, toggleSize, toggleSize);
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    var prop = dataProp.GetArrayElementAtIndex(index);
                    Rect toggleRect = new(rowRect.x + x * toggleSize, rowRect.y, toggleSize, toggleSize);
                    prop.boolValue = EditorGUI.Toggle(toggleRect, prop.boolValue);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}