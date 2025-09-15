using UnityEngine;
using UnityEditor;

public static class MipMapBiasMenu
{
    public static Object[] selection;

    public const float defaultBias = 0.0f;
    public const float standardBias = -0.5f;
    public const float highBias = -1.0f;


    [MenuItem("Tools/MipMaps/Correct MipMap Bias - Standard", true)]
    public static bool ValidateStandardBias()
    {
        selection = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        return (selection.Length > 0);
    }

    [MenuItem("Tools/MipMaps/Correct MipMap Bias - Standard", false)]
    public static void StandardBias()
    {
        foreach (Object texture in selection)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            (AssetImporter.GetAtPath(path) as TextureImporter).mipMapBias = standardBias;
            AssetDatabase.ImportAsset(path);
        }
    }



    [MenuItem("Tools/MipMaps/Correct MipMap Bias - High", true)]
    public static bool ValidateHighBias()
    {
        selection = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        return (selection.Length > 0);
    }

    [MenuItem("Tools/MipMaps/Correct MipMap Bias - High", false)]
    public static void HighBias()
    {
        foreach (Object texture in selection)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            (AssetImporter.GetAtPath(path) as TextureImporter).mipMapBias = highBias;
            AssetDatabase.ImportAsset(path);
        }
    }



    [MenuItem("Tools/MipMaps/Return MipMap Bias", true)]
    public static bool ValidateReturnBias()
    {
        selection = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        return (selection.Length > 0);
    }

    [MenuItem("Tools/MipMaps/Return MipMap Bias", false)]
    public static void ReturnBias()
    {
        foreach (Object texture in selection)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            (AssetImporter.GetAtPath(path) as TextureImporter).mipMapBias = defaultBias;
            AssetDatabase.ImportAsset(path);
        }
    }
}