using System;
using System.Collections;
using System.Collections.Generic;
using HapticPatterns;
using HapticPatterns.Samples;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HapticPatterns.Samples
{
    public class HolsterPlayPattern : MonoBehaviour
    {
        private const float CooldownTime = .5f;
        
        public HapticPattern pattern;
        public HapticPatternVisualiser patternVisualiser;
        public float handSearchRadius;

        private float _lastPlayTime;
        
        private void Start()
        {
            patternVisualiser.pattern = pattern;
        }

        //This is called from holster select event
        public void PlayPatternOverTime()
        {
            if (Time.time - _lastPlayTime < CooldownTime)
                return;
            
            XRDirectInteractor hand = SearchForHand();

            if (hand == null)
                return;

            pattern.PlayOverTime(hand);
            patternVisualiser.PlayOverTime();

            _lastPlayTime = Time.time;
        }

        private XRDirectInteractor SearchForHand()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, handSearchRadius);

            foreach (Collider col in cols)
            {
                XRDirectInteractor hand = col.transform.GetComponent<XRDirectInteractor>();

                if (hand != null)
                    return hand;
            }

            return null;
        }
    }
}
