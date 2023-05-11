using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using HapticPatterns.Input;

namespace HapticPatterns.Samples
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class TactileTriggerPattern : MonoBehaviour
    {
        public HapticPattern gradualPattern;
        public HapticPatternVisualiser patternVisualiser;

        private XRIDefaultInputActions _input;
        private XRGrabInteractable _grabInteractable;
        
        private void Start()
        {
            _grabInteractable = GetComponent<XRGrabInteractable>();
            _input = new XRIDefaultInputActions();
            _input.Enable();
            
            patternVisualiser.pattern = gradualPattern;
        }

        private void Update()
        {
            if (!_grabInteractable.isSelected)
                return;
            if (_grabInteractable.interactorsSelecting[0] is not XRDirectInteractor)
                return;
            
            XRDirectInteractor hand = (XRDirectInteractor)_grabInteractable.interactorsSelecting[0];
            float trigger01 = GetTriggerValue();
            
            if(trigger01 > 0.001f)
                //Progressively play pattern, with change in trigger value over Time.deltaTime (until next frame)
                gradualPattern.PlayGradually(_grabInteractable, trigger01);
        
            patternVisualiser.pointOnTimeline = trigger01;
        }

        private float GetTriggerValue()
        {
            float leftTrigger = _input.XRILeftHandInteraction.ActivateValue.ReadValue<float>();
            float rightTrigger = _input.XRIRightHandInteraction.ActivateValue.ReadValue<float>();

            //Just return the largest value on both hands, since the awful new input system doesn't
            //document how to access input on only the selecting controller ):
            //TODO improve once i figure out how
            return Mathf.Max(leftTrigger, rightTrigger);  
        }
    }
}
