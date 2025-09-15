using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif

namespace Breaddog.Extensions
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class FromResourcesAttribute : PropertyAttribute
    {
        public Type ResourceType { get; private set; }
        public bool ShowPreview { get; private set; }

        public FromResourcesAttribute(Type resourceType = null, bool showPreview = true)
        {
            ResourceType = resourceType;
            ShowPreview = showPreview;
        }
    }


#if UNITY_EDITOR
    public class ResourcePathDropdown : AdvancedDropdown
    {
        private readonly Action<string> onSelect;
        private readonly List<string> resourcePaths;

        private const string subDirectory = "Data";

        public ResourcePathDropdown(AdvancedDropdownState state, SerializedProperty property, Type resourceType)
            : base(state)
        {
            this.onSelect = (path) =>
            {
                property.stringValue = $"{subDirectory}/{path}";
                property.serializedObject.ApplyModifiedProperties();
            };

            resourcePaths = new List<string>();

            string predicate = resourceType != null ? $"t:{resourceType.Name}" : string.Empty;
            string[] guids = AssetDatabase.FindAssets(predicate);
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.Contains($"/Resources/{subDirectory}/")) continue;

                int idx = path.IndexOf($"/Resources/{subDirectory}/") + $"/Resources/{subDirectory}/".Length;
                string relative = path.Substring(idx);
                string noExt = Path.ChangeExtension(relative, null);
                resourcePaths.Add(noExt);
            }

            resourcePaths.Sort();
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem($"Resources/{subDirectory}");

            foreach (string path in resourcePaths)
            {
                var parts = path.Split('/');
                AdvancedDropdownItem parent = root;

                for (int i = 0; i < parts.Length; i++)
                {
                    var found = parent.children.FirstOrDefault(c => c.name == parts[i]);
                    if (found == null)
                    {
                        var newItem = new AdvancedDropdownItem(parts[i]);
                        parent.AddChild(newItem);
                        parent = newItem;
                    }
                    else
                    {
                        parent = found;
                    }

                    if (i == parts.Length - 1)
                        parent.name = path;
                }
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            onSelect?.Invoke(item.name);
        }
    }


    [CustomPropertyDrawer(typeof(FromResourcesAttribute))]
    public class FromResourcesDrawer : PropertyDrawer
    {
        private AdvancedDropdownState dropdownState;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (dropdownState == null)
                dropdownState = new AdvancedDropdownState();

            EditorGUI.BeginProperty(position, label, property);

            if (attribute is FromResourcesAttribute typedAttr)
            {
                const float previewWidth = 110f;
                const float spacing = -12.5f;
                float labelWidth = EditorGUIUtility.labelWidth;

                bool showPreview = typedAttr.ShowPreview;
                float previewOffset = showPreview ? (previewWidth + spacing) : 0f;

                float fieldWidth = position.width - labelWidth - previewOffset;

                Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
                Rect buttonRect = new Rect(labelRect.xMax, position.y, fieldWidth, position.height);
                Rect previewRect = new Rect(buttonRect.xMax + spacing, position.y, previewWidth, position.height);

                EditorGUI.LabelField(labelRect, label);

                if (GUI.Button(buttonRect, string.IsNullOrEmpty(property.stringValue) ? "Select Resource" : property.stringValue, EditorStyles.popup))
                {
                    var dropdown = new ResourcePathDropdown(dropdownState, property, typedAttr.ResourceType);
                    dropdown.Show(buttonRect);
                }

                if (showPreview && !string.IsNullOrEmpty(property.stringValue) && typedAttr.ResourceType != null)
                {
                    UnityEngine.Object previewObj = Resources.Load(property.stringValue, typedAttr.ResourceType);
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUI.ObjectField(previewRect, GUIContent.none, previewObj, typedAttr.ResourceType, false);
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }

#endif
}
