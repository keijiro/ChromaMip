using UnityEngine;
using UnityEditor;
using System.Collections;

class ChromaMipProcessor : AssetPostprocessor
{
    static float RGB_Y(Color rgb)
    {
        return 0.299f * rgb.r + 0.587f * rgb.g + 0.114f * rgb.b;
    }

    static float RGB_Cb(Color rgb)
    {
        return -0.168736f * rgb.r -0.331264f * rgb.g + 0.5f * rgb.b;
    }

    static float RGB_Cr(Color rgb)
    {
        return 0.5f * rgb.r - 0.418688f * rgb.g - 0.081312f * rgb.b;
    }

    static float RGB_Ya(Color rgb)
    {
        if (rgb.a < 0.5f)
            return 0;
        else
            return RGB_Y(rgb) * 255 / 256 + 1.0f / 256;
    }

    void OnPreprocessTexture()
    {
        var importer = assetImporter as TextureImporter;

        if (!assetPath.EndsWith("CM.png")) return;

        importer.textureType = TextureImporterType.GUI;
        importer.textureFormat = TextureImporterFormat.RGBA32;
        importer.mipmapEnabled = true;
    }

    void OnPostprocessTexture(Texture2D texture)
    {
        var importer = assetImporter as TextureImporter;

        if (!assetPath.EndsWith("CM.png")) return;

        var hasAlpha = importer.DoesSourceTextureHaveAlpha();

        var tw = texture.width;
        var th = texture.height;

        var pixels = texture.GetPixels(0);
        var index = 0;

        if (hasAlpha)
        {
            for (var iy = 0; iy < th; iy++)
                for (var ix = 0; ix < tw; ix++, index++)
                    pixels[index].a = RGB_Ya(pixels[index]);
        }
        else
        {
            for (var iy = 0; iy < th; iy++)
                for (var ix = 0; ix < tw; ix++, index++)
                    pixels[index].a = RGB_Y(pixels[index]);
        }

        texture.SetPixels(pixels, 0);

        pixels = texture.GetPixels(1);
        index = 0;

        for (var iy = 0; iy < th / 2; iy++)
            for (var ix = 0; ix < tw / 2; ix++, index++)
                pixels[index].a = RGB_Cr(pixels[index]) + 0.5f;

        texture.SetPixels(pixels, 1);

        pixels = texture.GetPixels(2);
        index = 0;

        for (var iy = 0; iy < th / 4; iy++)
            for (var ix = 0; ix < tw / 4; ix++, index++)
                pixels[index].a = RGB_Cb(pixels[index]) + 0.5f;

        texture.SetPixels(pixels, 2);

        EditorUtility.CompressTexture(texture, TextureFormat.Alpha8, TextureCompressionQuality.Best);
    }
}
