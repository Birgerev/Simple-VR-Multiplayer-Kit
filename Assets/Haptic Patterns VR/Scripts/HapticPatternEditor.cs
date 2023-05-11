#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace HapticPatterns
{
    [CustomEditor(typeof(HapticPattern))]
    [CanEditMultipleObjects]
    public class HapticPatternEditor : Editor
    {
        private HapticPattern _targetInfo;
        private float _customDuration;

        public void OnEnable()
        {
            if (_targetInfo == null)
            {
                _targetInfo = target as HapticPattern;
            }

            _customDuration = _targetInfo.hapticCurve.keys[^1].time;
        }

        public override void OnInspectorGUI()
        {
            // Remember to display the other GUI from the object if you want to see all its normal properties*/
            base.OnInspectorGUI();

            if (_targetInfo.secondaryHandStrengthMultiplier == 0)
                EditorGUILayout.LabelField("Disabled");
            else
                EditorGUILayout.Space(20);

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Pattern Timeline");

            AnimationCurve newCurve = EditorGUILayout.CurveField(_targetInfo.hapticCurve, Color.red,
                new Rect(0, 0, _targetInfo.hapticCurve.keys[^1].time, 1), GUILayout.MinHeight(200));

            //Create custom duration field
            _customDuration = Mathf.Clamp(EditorGUILayout.FloatField("Pattern Duration", _customDuration), 0.005f,
                Mathf.Infinity);

            //Make sure curve always as start and end keyframe
            if (newCurve.keys.Length < 2)
                newCurve.AddKey(0, 0);

            //Lock start keyframe to 0 time
            newCurve.MoveKey(0, new Keyframe(0, newCurve.keys[0].value));

            //Merge key frames if the two last ones are too close
            if (_customDuration - newCurve.keys[^2].time < .001f)
                newCurve.RemoveKey(newCurve.keys.Length - 1);

            //Lock last keyframe to custom length time
            newCurve.MoveKey(newCurve.keys.Length - 1, new Keyframe(_customDuration, newCurve.keys[^1].value));


            _targetInfo.hapticCurve = newCurve;
        }

        private void OnDisable()
        {
            EditorUtility.SetDirty(_targetInfo);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif