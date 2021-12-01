//Zenith Code 2014
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public static class RenderingHighlighterUtility
{
    private const string ASSET_PATH = "Assets/RenderingOrderHighlighterTool/Resources/RenderHighlightersData.asset";
    private const string DEFAULT_ICON_ASSET_PATH = "Assets/RenderingOrderHighlighterTool/Icons/DefaultIcon.tif";
    private const string DEFAULT_LAYER_NAME = "Default";
    private static RenderingHighlighters _renderingHighlightersDataFile;

    #region public API
    public static RenderingHighlighters RenderingHighlightersDataFile
    {
        get
        {
            if (_renderingHighlightersDataFile == null)
            {
                _renderingHighlightersDataFile = LoadAssetFile();
            }
            return _renderingHighlightersDataFile ?? (_renderingHighlightersDataFile = CreateNew());
        }
    }


    public static RenderingHighlighter GetRenderingHighlighterForLayer(string sortLayer)
    {
        foreach (var renderingHighlighter in RenderingHighlightersDataFile.RenderingHighlightersList)
        {
            if (renderingHighlighter.LayerName.Equals(sortLayer))
            {
                return renderingHighlighter;
            }
        }
        return GetDefaultRenderHighlighter(sortLayer);
    }


    public static void ResyncAssetFile()
    {
        string[] sortingLayerNames = GetSortingLayerNames();

        //Fill them up with defaults
        var newRenderingHighlighters = new List<RenderingHighlighter>(sortingLayerNames.Length);
        for (int i = 0; i < sortingLayerNames.Length; i++)
        {
            newRenderingHighlighters.Add(GetDefaultRenderHighlighter(sortingLayerNames[i]));
        }

        var renderingHighlighters = RenderingHighlightersDataFile.RenderingHighlightersList;
        for (int i = 0; i < newRenderingHighlighters.Count - 1; i++)
        {
            RenderingHighlighter newRenderingHighlighter = newRenderingHighlighters[i];
            foreach (RenderingHighlighter renderingHighlighter in renderingHighlighters)
            {
                if (newRenderingHighlighter.LayerName == renderingHighlighter.LayerName)
                {
                    if (renderingHighlighter.layerTexture == null)
                    {
                        renderingHighlighter.layerTexture = GetCorrectTextureForLayer(newRenderingHighlighter.LayerName);
                    }
                    newRenderingHighlighters[i] = renderingHighlighter;
                    break;
                }
            }
        }

        _renderingHighlightersDataFile.RenderingHighlightersList = newRenderingHighlighters;
    }


    public static List<RendererObject> GetRenderersObjectList()
    {
        var rendererObjects = new List<RendererObject>();

        //Process all sprite renderers
        var allSpriteRenderers = Resources.FindObjectsOfTypeAll(typeof(SpriteRenderer)) as SpriteRenderer[];
        if (allSpriteRenderers != null)
        {
            foreach (SpriteRenderer currentRenderer in allSpriteRenderers)
            {
                string sortLayer = currentRenderer.sortingLayerName;
                if (string.IsNullOrEmpty(sortLayer))
                {
                    sortLayer = DEFAULT_LAYER_NAME;
                }
                rendererObjects.Add(new RendererObject(currentRenderer.gameObject.GetInstanceID(), sortLayer,
                                                       currentRenderer.sortingOrder));
            }
        }


        //Process all vfx sorters
		var allVfxSorters = Resources.FindObjectsOfTypeAll(typeof(SortingLayer)) as SortingLayer[];
        if (allVfxSorters != null)
        {
			foreach (SortingLayer sorter in allVfxSorters)
            {
				var vfxSorter = sorter.GetComponent<SortingLayer>();
                string sortLayer = vfxSorter.sortingLayer;
                if (string.IsNullOrEmpty(sortLayer))
                {
                    sortLayer = DEFAULT_LAYER_NAME;
                }
                rendererObjects.Add(new RendererObject(sorter.gameObject.GetInstanceID(), sortLayer, vfxSorter.sortingOrder));
            }
        }
        return rendererObjects;
    }


    public static int[] GetObjectIDsOnLayer(string sortingLayerName)
    {
        var renderersObjectList = GetRenderersObjectList();
        return (
            from rendererObject
            in renderersObjectList
            where rendererObject.sortingLayerName == sortingLayerName
            select rendererObject.instanceId)
                .ToArray();
    }


    #region GetSortingLayers
    //Thanks to: http://answers.unity3d.com/questions/585108/how-do-you-access-sorting-layers-via-scripting.html
    public static string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }
    #endregion
    #endregion



    #region Helpers
    private static RenderingHighlighter GetDefaultRenderHighlighter(string sortLayer)
    {
        //If this layer is not available in tool yet, allow default values.
        var defaultRenderingHighlighter = new RenderingHighlighter
        {
            LayerName = sortLayer,
            sortingOrderTextColor = GetCorrectTextColorForLayer(sortLayer),
            layerTexture = GetCorrectTextureForLayer(sortLayer)
        };
        return defaultRenderingHighlighter;
    }


    public static RenderingHighlighters LoadAssetFile()
    {
        _renderingHighlightersDataFile = AssetDatabase.LoadAssetAtPath(ASSET_PATH, typeof(RenderingHighlighters)) as RenderingHighlighters;
        if (_renderingHighlightersDataFile != null)
        {
            ResyncAssetFile();
        }
        return _renderingHighlightersDataFile;
    }


    private static RenderingHighlighters CreateNew()
    {
        var highlighters = ScriptableObject.CreateInstance<RenderingHighlighters>();
        highlighters.RenderingHighlightersList = new List<RenderingHighlighter>();

        string[] sortingLayerNames = GetSortingLayerNames();

        for (int i = 0; i < sortingLayerNames.Length - 1; i++)
        {
            highlighters.RenderingHighlightersList.Add(GetDefaultRenderHighlighter(sortingLayerNames[i]));
        }

        //	Create the asset.
        AssetDatabase.CreateAsset(highlighters, ASSET_PATH);
        _renderingHighlightersDataFile = highlighters;

        //SaveAssetFile();
        return _renderingHighlightersDataFile;
    }


    private static void SaveAssetFile()
    {
        EditorUtility.SetDirty(_renderingHighlightersDataFile);
        AssetDatabase.SaveAssets();
    }


    private static Texture2D GetDefaultTexture()
    {
        var defaultTexture = AssetDatabase.LoadAssetAtPath(DEFAULT_ICON_ASSET_PATH, typeof(Texture2D)) as Texture2D;
        if (defaultTexture == null)
        {
            Debug.LogError(string.Format("Default icon cannot be found!!"));
        }
        return defaultTexture;
    }


    private static Texture2D GetCorrectTextureForLayer(string sortLayer)
    {
        if (_renderingHighlightersDataFile != null)
        {
            foreach (var renderingHighlighter in _renderingHighlightersDataFile.RenderingHighlightersList)
            {
                if (renderingHighlighter.LayerName.Equals(sortLayer))
                {
                    if (renderingHighlighter.layerTexture == null)
                    {
                        return GetDefaultTexture();
                    }
                    return renderingHighlighter.layerTexture;
                }
            }
        }
        return GetDefaultTexture();
    }


    private static Color GetCorrectTextColorForLayer(string sortLayer)
    {
        if (_renderingHighlightersDataFile != null)
        {
            foreach (var renderingHighlighter in _renderingHighlightersDataFile.RenderingHighlightersList)
            {
                if (renderingHighlighter.LayerName.Equals(sortLayer))
                {
                    return renderingHighlighter.sortingOrderTextColor;
                }
            }
        }
        return Color.white;
    }
    #endregion
}
