using System.Collections;
using System.Collections.Generic;
using HapticPatterns;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HapticPatterns
{
    public class ExampleOverTime : MonoBehaviour
    {
        public HapticPattern pattern;

        public void SomeEvent()
        {
            XRGrabInteractable grabbableObject = GetComponent<XRGrabInteractable>();

            pattern.PlayOverTime(grabbableObject);
        }
    }
}