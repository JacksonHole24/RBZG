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
            curve.canBeDeleted = true;
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
                if (_curve.hasBeenSaved || !_curve.canBeDeleted)
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

            if (GUILayout.Button("Duplicate Curve", GUILayout.Width(150), GUILayout.Height(20)))
            {
                Curve curve = new Curve();
                curve.curve1 = new AnimationCurve(_curve.curve1.keys);
                curve.canBeDeleted = true;
                curves.Add(curve);
            }

            if (GUILayout.Button("Reverse X Curve", GUILayout.Width(150), GUILayout.Height(20)))
            {
                Curve curve = new Curve();
                curve.curve1 = new AnimationCurve(_curve.curve1.keys);
                curve.canBeDeleted = true;
                Keyframe[] originalKeyframes = curve.curve1.keys;
                int length = originalKeyframes.Length;
                Keyframe[] reversedKeyframes = new Keyframe[length];

                // Reverse the keyframe values
                for (int i = 0; i < length; i++)
                {
                    Keyframe originalKeyframe = originalKeyframes[i];
                    Keyframe reversedKeyframe = new Keyframe(originalKeyframe.time, originalKeyframe.value);
                    reversedKeyframe.inTangent = -originalKeyframe.inTangent;
                    reversedKeyframe.outTangent = -originalKeyframe.outTangent;
                    reversedKeyframes[length - 1 - i] = reversedKeyframe;
                }

                // Adjust the keyframe positions
                float firstTime = reversedKeyframes[0].time;
                float lastTime = reversedKeyframes[length - 1].time;
                float timeRange = lastTime - firstTime;

                for (int i = 0; i < length; i++)
                {
                    Keyframe reversedKeyframe = reversedKeyframes[i];
                    reversedKeyframe.time = firstTime + (timeRange - reversedKeyframe.time);
                    reversedKeyframes[i] = reversedKeyframe;
                }

                curve.curve1.keys = reversedKeyframes;

                curves.Add(curve);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(_curve.smoothAmount.ToString(), EditorStyles.boldLabel, GUILayout.Width(40), GUILayout.Height(20));
            _curve.smoothAmount = GUILayout.HorizontalSlider(_curve.smoothAmount, 10f, 100f);
            if (GUILayout.Button("Smoother Curve", GUILayout.Width(150), GUILayout.Height(20)))
            {
                Curve curve = new Curve();
                curve.curve1 = new AnimationCurve(_curve.curve1.keys);
                curve.canBeDeleted = true;

                float smoothAmount = _curve.smoothAmount * 0.01f; // Scale the smooth amount to a 0-1 range

                Keyframe[] keyframes = curve.curve1.keys;
                Keyframe[] adjustedKeyframes = new Keyframe[keyframes.Length];

                // Copy keyframes to adjustedKeyframes
                for (int i = 0; i < keyframes.Length; i++)
                {
                    adjustedKeyframes[i] = keyframes[i];
                }

                // Smooth keyframes using Catmull-Rom spline interpolation
                for (int i = 1; i < adjustedKeyframes.Length - 1; i++)
                {
                    Vector2 p0 = new Vector2(adjustedKeyframes[i - 1].time, adjustedKeyframes[i - 1].value);
                    Vector2 p1 = new Vector2(adjustedKeyframes[i].time, adjustedKeyframes[i].value);
                    Vector2 p2 = new Vector2(adjustedKeyframes[i + 1].time, adjustedKeyframes[i + 1].value);

                    // Calculate tangents using Catmull-Rom spline formula
                    float smoothingFactor = Mathf.Lerp(0.5f, 1f, smoothAmount); // Adjust the smoothing factor
                    Vector2 tangentIn = (p2 - p0) * (0.5f * smoothAmount * smoothingFactor);
                    Vector2 tangentOut = -tangentIn;

                    // Update the tangents and weights of the current keyframe
                    adjustedKeyframes[i].inTangent = tangentIn.y / tangentIn.x;
                    adjustedKeyframes[i].outTangent = tangentOut.y / tangentOut.x;
                    adjustedKeyframes[i].inWeight = smoothAmount;
                    adjustedKeyframes[i].outWeight = smoothAmount;
                }

                curve.curve1.keys = adjustedKeyframes;

                curves.Add(curve); // Add curve to the curves list
            }


            GUILayout.EndHorizontal();
        }
        EditorGUILayout.LabelField("Copy a curve out from the curve editor data to use it", EditorStyles.boldLabel, GUILayout.Width(400), GUILayout.Height(20));
    }
}
