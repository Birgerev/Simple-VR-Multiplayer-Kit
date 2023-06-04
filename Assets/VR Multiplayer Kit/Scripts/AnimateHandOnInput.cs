using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VRMultiplayerStarterKit
{
    
    /// <summary>
    /// Script that translates player input to animation values
    /// </summary>
    public class AnimateHandOnInput : MonoBehaviour
    {
        public InputActionProperty pinchAnimationAction;
        public InputActionProperty gripAnimationAction;
        public Animator handAnimator;

        // Update is called once per frame
        void Update()
        {
            //Read input float values
            float triggerValue = pinchAnimationAction.action.ReadValue<float>();
            float gripValue = gripAnimationAction.action.ReadValue<float>();

            //Set animator values
            handAnimator.SetFloat("Trigger", triggerValue);
            handAnimator.SetFloat("Grip", gripValue);
        }
    }
}
