using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace HapticPatterns.Samples
{
    public class ComponentsEnabledWhenHeld : MonoBehaviour
    {
        public XRBaseInteractable mustBeHeld;
        [Space] public Behaviour[] dependentComponents;

        // Update is called once per frame
        void Update()
        {
            bool isHeld = false;

            //Check if object is being held by a hand
            if (mustBeHeld.isSelected)
                if (mustBeHeld.interactorsSelecting[0].transform.GetComponent<ActionBasedController>() != null)
                    isHeld = true;

            //Update active state
            foreach (Behaviour component in dependentComponents)
            {
                component.enabled = isHeld;
            }
        }
    }
}