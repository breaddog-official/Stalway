using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
#endif

namespace Breaddog.Extensions
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class FromAddressablesAttribute : PropertyAttribute
    {
        public Type AssetType { get; private set; }
        public bool ShowPreview { get; private set; }

        public FromAddressablesAttribute(Type assetType = null, bool showPreview = true)
        {
            AssetType = assetType;
            ShowPreview = showPreview;
        }
    }


#if UNITY_EDITOR
    public class AddressablePathDropdown : AdvancedDropdown
    {
        private readonly Action<string> onSelect;
        private readonly List<(string key, string path)> entries;

        public AddressablePathDropdown(AdvancedDropdownState state, SerializedProperty property, Type assetType)
            : base(state)
        {
            onSelect = (key) =>
            {
                property.stringValue = key;
                property.serializedObject.ApplyModifiedProperties();
            };

            entries = new List<(string, string)>();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return;

            foreach (var group in settings.groups)
            {
                foreach (var entry in group.entries)
                {
                    string path = AssetDatabase.GUIDToAssetPath(entry.guid);
                    if (string.IsNullOrEmpty(path)) continue;

                    if (assetType != null)
                    {
                        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(path, assetType);
                        if (asset == null) continue;
                    }

                    entries.Add((entry.address, path));
                }
            }

            entries.Sort((a, b) => a.key.CompareTo(b.key));
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Addressables");

            foreach (var (key, _) in entries)
            {
                var parts = key.Split('/');
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
                        parent.name = key;
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
    [CustomPropertyDrawer(typeof(FromAddressablesAttribute))]
    public class FromAddressablesDrawer : PropertyDrawer
    {
        private AdvancedDropdownState dropdownState;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (dropdownState == null)
                dropdownState = new AdvancedDropdownState();

            EditorGUI.BeginProperty(position, label, property);

            if (attribute is FromAddressablesAttribute typedAttr)
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

                if (GUI.Button(buttonRect, string.IsNullOrEmpty(property.stringValue) ? "Select Asset" : property.stringValue, EditorStyles.popup))
                {
                    var dropdown = new AddressablePathDropdown(dropdownState, property, typedAttr.AssetType);
                    dropdown.Show(buttonRect);
                }

                if (showPreview && !string.IsNullOrEmpty(property.stringValue) && typedAttr.AssetType != null)
                {
                    string address = property.stringValue;
                    var settings = AddressableAssetSettingsDefaultObject.Settings;
                    var entry = settings?.FindAssetEntry(address);
                    string assetPath = entry != null ? AssetDatabase.GUIDToAssetPath(entry.guid) : null;

                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        UnityEngine.Object previewObj = AssetDatabase.LoadAssetAtPath(assetPath, typedAttr.AssetType);
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EditorGUI.ObjectField(previewRect, GUIContent.none, previewObj, typedAttr.AssetType, false);
                        }
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }

#endif
}
