using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HapticPatterns
{
    [CreateAssetMenu(fileName = "Vibration Pattern", menuName = "XR/Haptic Pattern")]
    public class HapticPattern : ScriptableObject
    {
        [HideInInspector] public AnimationCurve hapticCurve = AnimationCurve.Constant(0, 1f, 1);
        [Space] [Space] [Space] 
        [Range(0, 1)] public float secondaryHandStrengthMultiplier = 1f;

        private float _gradualPlaybackLastValue;
        
        /// <summary>
        /// Plays the haptic curve  over time on a specified XR Device
        /// </summary>
        /// <param name="device">Specified XR Device</param>
        /// <param name="strengthMultiplier">OPTIONAL strength factor for the vibrations</param>
        public async void PlayOverTime(XRBaseControllerInteractor device, float strengthMultiplier = 1)
        {
            const float curveSubdivisionDuration = .01f;
            
            float endTime = GetDuration();
            float time = 0;

            while (time < endTime)
            {
                PlayPeakInInterval(device,
                    new Interval(time, 
                        time + curveSubdivisionDuration), 
                    curveSubdivisionDuration,
                    strengthMultiplier);
                
                //Complex time handling code because Task.Delay() waits about 10ms too long
                Stopwatch watch = new Stopwatch();
                watch.Start();
                await Task.Delay((int)(curveSubdivisionDuration*1000));
                watch.Stop();
                time += (float)watch.Elapsed.TotalSeconds;
            }
        }

        /// <summary>
        /// Plays the haptic curve over time on all devices holding the specified
        /// XR Interactable or related interactables (Children and Parents).
        /// Vibration strength on the secondary hand can be altered in the Editor.
        /// </summary>
        /// <param name="heldObject">The interactable that will play vibrations on holding hands</param>
        public void PlayOverTime(XRBaseInteractable heldObject)
        {
            //Primary hand haptic
            XRBaseControllerInteractor primaryHand = AttemptToFindPrimaryHand(heldObject);
            if (primaryHand)
                PlayOverTime(primaryHand);
            
            //Secondary hand haptic, if enabled
            if (secondaryHandStrengthMultiplier > 0)
            {
                XRBaseControllerInteractor secondaryHand = AttemptToFindSecondaryHand(heldObject);

                if(secondaryHand)
                    PlayOverTime(secondaryHand, secondaryHandStrengthMultiplier);
            }
        }

        /// <summary>
        /// Recommended to be called every frame inside of Update(). Used to gradually play
        /// a haptic curve with at a specific point in time.
        /// </summary>
        /// <param name="device">The device to play haptic on</param>
        /// <param name="timelinePoint">Current time of haptic this frame, recommended to use values between 0 and 1</param>
        /// <param name="strengthMultiplier">OPTIONAL strength factor for the vibrations</param>
        public void PlayGradually(XRBaseControllerInteractor device, float timelinePoint, float strengthMultiplier = 1)
        {
            //Haptic Pattern Interval Playback
            PlayPeakInInterval(
                device, 
                new Interval(_gradualPlaybackLastValue, timelinePoint),
                Time.deltaTime,
                strengthMultiplier);

            _gradualPlaybackLastValue = timelinePoint;
        }

        /// <summary>
        /// Recommended to be called every frame inside of Update(). Used to gradually play
        /// a haptic curve with at a specific point in time. 
        /// </summary>
        /// <param name="heldObject">The interactable that will play vibrations on holding hands</param>
        /// <param name="timelinePoint">Current time of haptic this frame, recommended to use values between 0 and 1</param>
        public void PlayGradually(XRBaseInteractable heldObject, float timelinePoint)
        {
            //Primary hand haptic
            XRBaseControllerInteractor primaryHand = AttemptToFindPrimaryHand(heldObject);
            if (primaryHand)
                PlayGradually(primaryHand, timelinePoint);
            
            //Secondary hand haptic, if enabled
            if (secondaryHandStrengthMultiplier > 0)
            {
                XRBaseControllerInteractor secondaryHand = AttemptToFindSecondaryHand(heldObject);

                if(secondaryHand)
                    PlayGradually(secondaryHand, timelinePoint, secondaryHandStrengthMultiplier);
            }
        }
        
        /// <summary>
        /// Will only play the strongest vibration peak in haptic curve interval on a specified device.
        /// </summary>
        /// <param name="device">Device to play vibrations on</param>
        /// <param name="interval">Interval to play (start & end point)</param>
        /// <param name="playbackDuration">How long to play the vibration (in seconds)</param>
        /// <param name="strengthMultiplier">Optional vibration strength factor</param>
        private void PlayPeakInInterval(XRBaseControllerInteractor device, Interval interval, float playbackDuration, float strengthMultiplier = 1)
        {
            //Divide interval into x pieces and try to find peaks
            const int peakCheckIntervalDivisions = 20;
            float highestValueInInterval = 0;
            for (int i = 0; i < peakCheckIntervalDivisions; i++)
            {
                float peakDivisionTime = interval.start + (interval.GetDuration() / peakCheckIntervalDivisions) * i;
                float divisionValue = hapticCurve.Evaluate(peakDivisionTime);

                if (divisionValue > highestValueInInterval)
                    highestValueInInterval = divisionValue;
            }

            if (highestValueInInterval == 0)
                return;
            
            device.SendHapticImpulse(highestValueInInterval * strengthMultiplier, playbackDuration);
        }

        /// <summary>
        /// Get the total duration of the haptic curve
        /// </summary>
        /// <returns>Haptic curve duration (in seconds)</returns>
        public float GetDuration()
        {
            if (hapticCurve.keys.Length == 0)
                return 0;
            
            return hapticCurve.keys[^1].time;
        }
        
        #region Hand Retrieval
        private XRBaseControllerInteractor AttemptToFindPrimaryHand(XRBaseInteractable heldObject)
        {
            //Return if object isn't held
            if (heldObject.interactorsSelecting.Count == 0)
                return null;

            return heldObject.interactorsSelecting[0].transform.GetComponent<XRBaseControllerInteractor>();
        }

        private XRBaseControllerInteractor AttemptToFindSecondaryHand(XRBaseInteractable heldObject)
        {
            //Try to find secondary hand on same interactable
            if(heldObject.interactorsSelecting.Count == 2)
            {
                XRBaseControllerInteractor secondaryHand =
                    heldObject.interactorsSelecting[1].transform.GetComponent<XRBaseControllerInteractor>();

                if (secondaryHand != null)
                    return secondaryHand;
            }

            //Try to find secondary hand on related (parents and children's) interactables
            List<XRBaseInteractable> relatedInteractables = new List<XRBaseInteractable>();
            //Add interactables in children
            relatedInteractables.AddRange(heldObject.transform.GetComponentsInChildren<XRBaseInteractable>());
            //Add interactables in parents
            relatedInteractables.AddRange(heldObject.transform.GetComponentsInParent<XRBaseInteractable>());

            foreach (XRBaseInteractable relatedInteractable in relatedInteractables)
            {
                //Skip iterating interactable for primary hand
                if(relatedInteractable == heldObject)
                    continue;
                
                if(relatedInteractable.interactorsSelecting.Count > 0)
                {
                    XRBaseControllerInteractor secondaryHand =
                        relatedInteractable.interactorsSelecting[0].transform.GetComponent<XRBaseControllerInteractor>();

                    if (secondaryHand != null)
                        return secondaryHand;
                }
            }

            return null;
        }
        #endregion
    }
    
    public struct Interval
    {
        public float start;
        public float end;

        public Interval(float start, float end)
        {
            this.start = start;
            this.end = end;
        }

        public float GetDuration()
        {
            return end - start;
        }
    }
}