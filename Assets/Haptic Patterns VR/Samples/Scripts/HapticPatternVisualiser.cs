using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HapticPatterns.Samples
{
    public class HapticPatternVisualiser : MonoBehaviour
    {
        [HideInInspector]
        public HapticPattern pattern;
        [HideInInspector]
        public float pointOnTimeline;
        
        [FormerlySerializedAs("imageComponent")] [Header("Components")]
        public RawImage graphComponent;
        
        [Space] [Header("Graph Design")]
        public int pixelResolution;
        public Color lineColor = Color.red;
        
        [Space] [Header("Time Marker Design")]
        public int markerThickness = 3;
        public Color markerColor = Color.yellow;

        private bool _playingOverTime;
        
        // Update is called once per frame
        void Update()
        {
            if (_playingOverTime)
            {
                pointOnTimeline += Time.deltaTime;

                //If the whole pattern has been played
                if (pointOnTimeline > pattern.GetDuration())
                {
                    //Stop playing and reset time
                    _playingOverTime = false;
                    pointOnTimeline = 0;
                }
            }
            
            graphComponent.texture = GenerateVisualRepresentation();
        }

        public void PlayOverTime()
        {
            _playingOverTime = true;
            pointOnTimeline = 0;
        }

        private Texture2D GenerateVisualRepresentation()
        {
            if (pattern == null)
            {
                Debug.LogWarning("HapticPatternVisualiser '" + gameObject.name + "' has never been assigned a pattern (this is done through code)");
                return null;
            }

            Texture2D texture2D = new Texture2D(pixelResolution, pixelResolution);
            texture2D.filterMode = FilterMode.Point;
            
            AnimationCurve curve = pattern.hapticCurve;
            float curveDuration = pattern.GetDuration();
            
            int previousY = 0;
            for (int x = 0; x < pixelResolution; x++)
            {
                //Retrieve Y pixel
                float curveTime = (curveDuration / pixelResolution) * x;
                int currentY = (int)(pixelResolution * curve.Evaluate(curveTime));

                //Interpolate between previous and current y-pixel values
                int startY = Mathf.Min(previousY, currentY);
                int endY = Mathf.Max(previousY, currentY);
                for (int interpolatedY = startY; interpolatedY <= endY; interpolatedY++)
                {
                     if (interpolatedY < 0 || interpolatedY >= pixelResolution)
                         break;
                     
                     texture2D.SetPixel(x, interpolatedY, lineColor);
                }

                // Create final pixel
                previousY = currentY;
            }
            
            //Render Time Marker
            float time01 = pointOnTimeline / curveDuration;
            int markerX = (int)(time01 * pixelResolution);
            for (int y = 0; y < pixelResolution; y++)
            {
                //Offset x, n times for thickness
                for (int xOffset = 0; xOffset < markerThickness; xOffset++)
                {
                    texture2D.SetPixel(markerX + xOffset, y, markerColor);
                }
            }
            
            texture2D.Apply();

            return texture2D;
        }
    }
}