//Zenith Code 2014
using UnityEditor;

[CustomEditor(typeof(SortingLayer))]
public class SortingLayerEditor: Editor
{
    private SortingLayer _vfx;
    private int _index;

    private void OnEnable()
    {
		_vfx = (SortingLayer)target;
        //Get current layer
        for (int i = 0; i < _vfx.sortingLayers.Length; i++)
        {
            if (_vfx.sortingLayers[i] == _vfx.sortingLayer)
            {
                _index = i;
            }
        }
    }


    public override void OnInspectorGUI()
    {
        //Sorting layer
        _index = EditorGUILayout.Popup("Sorting layers", _index, _vfx.sortingLayers);
        _vfx.sortingLayer = _vfx.sortingLayers[_index];
        
        //Sorting order
        _vfx.sortingOrder = EditorGUILayout.IntField("Sort Order", _vfx.sortingOrder);
    }
}
