#if UNITY_EDITOR
using System.IO;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class BillboardCreator : MonoBehaviour
{
    public string savePath;
    public Vector2Int resolution;
    [Space]
    public bool ignoreX;
    public Camera cam;
    public Renderer obj;

    [Button]
    public void Prepare()
    {
        var bounds = obj.bounds;
        var extents = obj.bounds.extents;
        var depth = bounds.size.z * 0.5f + 1f;

        cam.orthographic = true;
        cam.orthographicSize = Mathf.Max(extents.x, extents.y);

        var pos = bounds.center - cam.transform.forward * depth;
        if (ignoreX)
            pos.x = 0f;

        cam.transform.position = pos;
        cam.transform.LookAt(bounds.center);
    }

    [Button]
    public void Capture()
    {
        var path = Path.Combine(savePath, "billboard.png");
        /*var rt = new RenderTexture(resolution.x, resolution.y, 24, RenderTextureFormat.ARGB32);
        rt.useMipMap = true;
        rt.autoGenerateMips = true;

        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;

        var tex = new Texture2D(resolution.x, resolution.y, TextureFormat.ARGB32, true);
        tex.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
        tex.Apply();

        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.Refresh();

        RenderTexture.active = null;
        cam.targetTexture = null;
        DestroyImmediate(tex);
        DestroyImmediate(rt);*/

        CaptureTransparentScreenshot(cam, resolution.x, resolution.y, path);
    }

    public static void CaptureTransparentScreenshot(Camera cam, int width, int height, string screengrabfile_path)
    {
        var bak_cam_targetTexture = cam.targetTexture;
        var bak_cam_clearFlags = cam.clearFlags;
        var bak_cam_color = cam.backgroundColor;
        var bak_RenderTexture_active = RenderTexture.active;

        var tex_white = new Texture2D(width, height, TextureFormat.ARGB32, false);
        var tex_black = new Texture2D(width, height, TextureFormat.ARGB32, false);
        var tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Must use 24-bit depth buffer to be able to fill background.
        var render_texture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        var grab_area = new Rect(0, 0, width, height);

        RenderTexture.active = render_texture;
        cam.targetTexture = render_texture;
        cam.clearFlags = CameraClearFlags.SolidColor;

        cam.backgroundColor = Color.black;
        cam.Render();
        tex_black.ReadPixels(grab_area, 0, 0);
        tex_black.Apply();

        cam.backgroundColor = Color.white;
        cam.Render();
        tex_white.ReadPixels(grab_area, 0, 0);
        tex_white.Apply();

        // Create Alpha from the difference between black and white camera renders
        for (int y = 0; y < tex_transparent.height; ++y)
        {
            for (int x = 0; x < tex_transparent.width; ++x)
            {
                float alpha = tex_white.GetPixel(x, y).r - tex_black.GetPixel(x, y).r;
                alpha = 1.0f - alpha;
                Color color;
                if (alpha == 0)
                {
                    color = Color.clear;
                }
                else
                {
                    color = tex_black.GetPixel(x, y) / alpha;
                }
                color.a = alpha;
                tex_transparent.SetPixel(x, y, color);
            }
        }

        // Encode the resulting output texture to a byte array then write to the file
        byte[] pngShot = ImageConversion.EncodeToPNG(tex_transparent);
        File.WriteAllBytes(screengrabfile_path, pngShot);

        cam.clearFlags = bak_cam_clearFlags;
        cam.targetTexture = bak_cam_targetTexture;
        cam.backgroundColor = bak_cam_color;
        RenderTexture.active = bak_RenderTexture_active;
        RenderTexture.ReleaseTemporary(render_texture);

        DestroyImmediate(tex_black);
        DestroyImmediate(tex_white);
        DestroyImmediate(tex_transparent);

        AssetDatabase.Refresh();
    }
}
#endif