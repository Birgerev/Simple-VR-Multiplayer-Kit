using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HapticPatterns
{
    public class ExampleGradual : MonoBehaviour
    {
        public HapticPattern pattern;
        public float gradualValue01;

        void Update()
        {
            XRGrabInteractable grabbableObject = GetComponent<XRGrabInteractable>();

            pattern.PlayGradually(grabbableObject, gradualValue01);
        }
    }
}