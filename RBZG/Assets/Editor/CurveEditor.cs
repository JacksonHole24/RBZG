using PlasticGui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CurveEditor : EditorWindow
{
    public CurveEditorData curveData;
    Curve curve = new Curve();
    List<Curve> curves = new List<Curve>();

    bool finishedEditingCurve = false;
    bool startedCreatingCurve = false;


    [MenuItem("Window/Curve Editor")]
    public static void OpenWindow()
    {
        CurveEditor window = EditorWindow.GetWindow<CurveEditor>("Curve Editor");
        window.Show();
    }

    private void CreateGUI()
    {
        curves.AddRange(curveData.curves);

        finishedEditingCurve = false;
        startedCreatingCurve = false;
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Create Curve", GUILayout.Width(150), GUILayout.Height(20)))
        {
            startedCreatingCurve = true;
            finishedEditingCurve = false;

            curve = new Curve();
            curve.canBeDeleted = false;
        }

        if (startedCreatingCurve && !finishedEditingCurve)
        {
            EditorGUILayout.LabelField("Curve Settings", EditorStyles.boldLabel);
            curve.curveType = (Curve.CurveType)EditorGUILayout.EnumPopup(curve.curveType);
            curve.startX = EditorGUILayout.FloatField("Start X" , curve.startX);
            curve.endX = EditorGUILayout.FloatField("End X", curve.endX);
            if (curve.curveType == Curve.CurveType.constant)
            {
                curve.y = EditorGUILayout.FloatField("Y Value", curve.y);
            }
            else
            {
                curve.startY = EditorGUILayout.FloatField("Start Y", curve.startY);
                curve.endY = EditorGUILayout.FloatField("End Y", curve.endY);
            }

            EditorGUILayout.LabelField("Graph Settings", EditorStyles.boldLabel);
            curve.curveColor = EditorGUILayout.ColorField("Curve Color", curve.curveColor);
            curve.minX = EditorGUILayout.FloatField("Min X", curve.minX);
            curve.minY = EditorGUILayout.FloatField("Min Y", curve.minY);
            curve.maxX = EditorGUILayout.FloatField("Max X", curve.maxX);
            curve.maxY = EditorGUILayout.FloatField("Max Y", curve.maxY);

            if (GUILayout.Button("Generate Curve", GUILayout.Width(150), GUILayout.Height(20)))
            {
                switch (curve.curveType)
                {
                    case Curve.CurveType.linear:
                        curve.curve1 = AnimationCurve.Linear(curve.startX, curve.startY, curve.endX, curve.endY);
                        break;
                    case Curve.CurveType.easeInOut:
                        curve.curve1 = AnimationCurve.EaseInOut(curve.startX, curve.startY, curve.endX, curve.endY);
                        break;
                    case Curve.CurveType.constant:
                        curve.curve1 = AnimationCurve.Constant(curve.startX, curve.startY, curve.y);
                        break;
                }

                curves.Add(curve);

                finishedEditingCurve = true;
            }
        }


        List<Curve> curve2 = new List<Curve>();
        curve2.AddRange(curves);
        foreach (Curve _curve in curve2)
        {
            if(_curve.curveColor.a == 0)
            {
                _curve.curveColor = Color.red;
            }
            _curve.curve1 = EditorGUILayout.CurveField(_curve.curve1, _curve.curveColor, new Rect(_curve.minX, _curve.minY, _curve.maxX, _curve.maxY));
            _curve.curveName = EditorGUILayout.TextField("Curve Name", _curve.curveName);
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Save Curve", GUILayout.Width(150), GUILayout.Height(20)))
            {
                Curve[] _curves = new Curve[curveData.curves.Length + 1];
                for (int i = 0; i < curveData.curves.Length; i++)
                {
                    _curves[i] = curveData.curves[i];
                }
                int newPos = _curves.Length - 1;
                _curves[newPos] = _curve;
                curveData.curves = _curves;
                _curve.hasBeenSaved = true;
            }
            if (GUILayout.Button("Delete Curve", GUILayout.Width(150), GUILayout.Height(20)))
            {
                curves.Remove(_curve);
                if(_curve.hasBeenSaved || _curve.canBeDeleted)
                {
                    Curve[] _curves = new Curve[curveData.curves.Length - 1];
                    for (int i = 0; i < curves.Count; i++)
                    {
                        _curves[i] = curves[i];
                    }
                    curveData.curves = _curves;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Duplicate Curve", GUILayout.Width(150), GUILayout.Height(20)))
            {
                Curve curve = new Curve();
                curve.curve1 = new AnimationCurve();
                curve.curve1 = _curve.curve1; 
                curve.canBeDeleted = false;
                curves.Add(curve);
            }
            if (GUILayout.Button("Reverse X Curve", GUILayout.Width(150), GUILayout.Height(20)))
            {
                Curve curve = new Curve();
                curve.curve1 = new AnimationCurve();
                curve.curve1 = _curve.curve1;
                curve.canBeDeleted = false;
                List<Keyframe> keys = new List<Keyframe>();
                keys.AddRange(curve.curve1.keys);

                for (int i = 0, p = curve.curve1.keys.Length - 1; i < curve.curve1.keys.Length; i++, p--)
                {
                    Keyframe keyframe = curve.curve1.keys[i];
                    keyframe.value = keys[p].value;
                    curve.curve1.MoveKey(i, keyframe);
                }

                curves.Add(curve);
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.LabelField("Copy a curve out from the curve editor data to use it", EditorStyles.boldLabel, GUILayout.Width(400), GUILayout.Height(20));
    }
}
