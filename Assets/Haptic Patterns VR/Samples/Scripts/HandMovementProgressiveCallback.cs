using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace HapticPatterns.Samples
{
    public class HandMovementProgressiveCallback : MonoBehaviour
    {
        public XRSimpleInteractable movementInteractable;
        public Vector3 handMovementDirectionSensitivity;
        public MovementActivationMode activationMode;
        
        [Space(20)]
        [Header("Feedback Components")]
        public Animator animator;
        public string animator01FieldName;
        [Space]
        public HapticPattern gradualHaptic;
        [Space] 
        public HapticPatternVisualiser curveVisualiser;

        private float _value01;
        private XRBaseControllerInteractor _hand;

        private Vector3 _handPositionLastFrame;
        
        // Update is called once per frame
        void Update()
        {
            _hand = GetHand();
            
            CalculateValue();

            if (_hand != null)
            {
                //Haptic Pattern Interval Playback
                if (activationMode == MovementActivationMode.Hover)
                    gradualHaptic.PlayGradually(_hand, _value01);
                
                if(activationMode == MovementActivationMode.Select)
                    gradualHaptic.PlayGradually(movementInteractable, _value01);
            }
            
            //Animation Playback
            animator.SetFloat(animator01FieldName, _value01);
            
            //Pattern Visualiser Playback
            curveVisualiser.pattern = gradualHaptic;
            curveVisualiser.pointOnTimeline = _value01;
        }

        private void CalculateValue()
        {
            if (_hand == null)
                _value01 = 0;
            else
                CalculateValueFromHandDeltaMovement();
        }

        private void CalculateValueFromHandDeltaMovement()
        {
            //Calculate hand movement change since last frame
            Vector3 deltaMovement = _hand.transform.position - _handPositionLastFrame;
            Vector3 deltaMovementInLocalSpace = transform.InverseTransformVector(deltaMovement);
            //Translate hand movement to bolt position change
            Vector3 deltaMovementInDirection = Vector3.Scale(-deltaMovementInLocalSpace, handMovementDirectionSensitivity);
            float deltaPressValue = deltaMovementInDirection.x + deltaMovementInDirection.y + deltaMovementInDirection.z;

            //Apply bolt position change
            _value01 = Mathf.Clamp01(_value01 + deltaPressValue);
        }

        private void LateUpdate()
        {
            if (movementInteractable.isHovered && _hand != null)
                _handPositionLastFrame = _hand.transform.position;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            if(movementInteractable == null || animator == null || animator01FieldName.Length == 0)
                Debug.LogWarning("Object "+ gameObject.name + " has un-configured values");
        }

        private XRBaseControllerInteractor GetHand()
        {
            XRBaseInteractor interactor = null;
            
            switch (activationMode)
            {
                case MovementActivationMode.Hover:
                    if(movementInteractable.interactorsHovering.Count > 0)
                        interactor = (XRBaseInteractor)movementInteractable.interactorsHovering[0];
                    break;
                case MovementActivationMode.Select:
                    if(movementInteractable.interactorsSelecting.Count > 0)
                        interactor = (XRBaseInteractor)movementInteractable.interactorsSelecting[0];
                    break;
            }

            if (interactor is XRBaseControllerInteractor hand)
                return hand;
            
            return null;
        }
    }

    public enum MovementActivationMode
    {
        Select,
        Hover,
    }
}