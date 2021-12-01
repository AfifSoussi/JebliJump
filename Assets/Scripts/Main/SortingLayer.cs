//Zenith Code 2014
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class SortingLayer : MonoBehaviour
{
    public int sortingOrder;
    public string[] sortingLayers;
    public string sortingLayer = "Default";

    private Renderer _localRenderer;

	#if UNITY_EDITOR
	void Awake()
	{
		sortingLayers = GetSortingLayerNames();
		_localRenderer = gameObject.GetComponent<Renderer>();
	}
	
	void Update()
	{
		SetRenderingLayerProperties();
	}
	
	
	#region GetSortingLayers
	//Thanks to: http://answers.unity3d.com/questions/585108/how-do-you-access-sorting-layers-via-scripting.html
	public string[] GetSortingLayerNames()
	{
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	}
	#endregion
#else
    void Awake()
    {
//        _localRenderer = gameObject.renderer;
 //       SetRenderingLayerProperties();
    }
#endif

	private void SetRenderingLayerProperties()
	{
		if (_localRenderer == null)
		{
			return;
		}
		_localRenderer.sortingLayerName = sortingLayer;
		_localRenderer.sortingOrder = sortingOrder;
	}
}