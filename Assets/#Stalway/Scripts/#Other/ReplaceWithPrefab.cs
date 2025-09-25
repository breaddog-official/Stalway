#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ReplaceWithPrefab : EditorWindow
{
    GameObject prefab;             // префаб для замены
    string nameFilter = "Example1_"; // фильтр по имени

    [MenuItem("Tools/Replace Objects With Prefab")]
    static void ShowWindow()
    {
        GetWindow<ReplaceWithPrefab>("Replace With Prefab");
    }

    void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab:", prefab, typeof(GameObject), false);
        nameFilter = EditorGUILayout.TextField("Name filter:", nameFilter);

        if (GUILayout.Button("Replace All"))
        {
            if (prefab == null)
            {
                Debug.LogError("Укажи префаб!");
                return;
            }

            ReplaceAll();
        }
    }

    void ReplaceAll()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where(o => o.scene.IsValid() && o.name.StartsWith(nameFilter))
                                                                                         .OrderBy(o => o.name) // сортируем по имени
                                                                                         .ToArray();
        int replacedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            string oldName = obj.name; // запоминаем имя

            // сохраняем transform
            Vector3 pos = obj.transform.position;
            Quaternion rot = obj.transform.rotation;
            Vector3 scale = obj.transform.localScale;
            Transform parent = obj.transform.parent;

            // создаём префаб
            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, obj.scene);

            // восстанавливаем transform
            newObj.transform.SetPositionAndRotation(pos, rot);
            newObj.transform.localScale = scale;
            if (parent != null)
                newObj.transform.SetParent(parent, true);

            // восстанавливаем имя
            newObj.name = oldName;

            Undo.RegisterCreatedObjectUndo(newObj, "Replace With Prefab");
            Undo.DestroyObjectImmediate(obj);

            replacedCount++;
        }

        Debug.Log($"Заменено {replacedCount} объектов на {prefab.name}.");
    }
}
#endif