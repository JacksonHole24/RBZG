using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Curve
{
    public string curveName;
    public AnimationCurve curve1 = new AnimationCurve();

    public enum CurveType
    {
        linear,
        easeInOut,
        constant
    }

    [HideInInspector] public CurveType curveType;
    [HideInInspector] public float startX;
    [HideInInspector] public float startY;
    [HideInInspector] public float endX;
    [HideInInspector] public float endY;
    [HideInInspector] public float minX;
    [HideInInspector] public float minY;
    [HideInInspector] public float maxX;
    [HideInInspector] public float maxY;
    [HideInInspector] public float y;
    [HideInInspector] public Color curveColor = Color.white;

    [HideInInspector] public bool isConstant;
    [HideInInspector] public bool hasBeenSaved = false;
    [HideInInspector] public bool canBeDeleted = true;
}
