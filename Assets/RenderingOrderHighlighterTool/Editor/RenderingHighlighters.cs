using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RenderingHighlighters : ScriptableObject
{
    [SerializeField]
    private List<RenderingHighlighter> _renderingHighlightersList;

    public List<RenderingHighlighter> RenderingHighlightersList
    {
        get { return _renderingHighlightersList; }
        set { _renderingHighlightersList = value; }
    }
}