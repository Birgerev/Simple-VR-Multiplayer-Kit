using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace VRMultiplayerStarterKit.Samples
{
    [RequireComponent(typeof(NetworkAnimator))]
    public class BowlingAlley : NetworkBehaviour
    {
        [Header("Pin properties")]
        public Transform[] pinSpawns;
        public GameObject pinPrefab;
        
        [Space] 
        [Header("Ball properties")]
        public Transform[] ballSpawns;
        public GameObject ballPrefab;
        
        [Space] 
        [Header("Behaviour properties")]
        public float animationDelaySeconds;
        public BoxCollider ballResetTrigger;

        private readonly List<GameObject> _trackedPins = new();
        private readonly List<GameObject> _trackedBalls = new();
        private bool _isResetting;

        private void Start()
        {
            if (!isServer)
                return;
            
            //Reset alley when game starts
            ResetAlley();
        }

        private void Update()
        {
            if (!isServer)
                return;
            
            //Reset range test, if all bowling balls have reached our defined trigger
            //& we aren't already resetting
            if(AllBallsHaveReachedTrigger() && !_isResetting)
                ResetAlley();
        }

        private bool AllBallsHaveReachedTrigger()
        {
            // Check for every tracked bowling ball
            foreach (GameObject ball in _trackedBalls)
            {
                // Check if the ball's position is outside the trigger
                if (!ballResetTrigger.bounds.Contains(ball.transform.position))
                    return false; // Return false if any ball is outside the bounds
            }
            
            //If all tracked balls were inside, return true
            return true;
        }


        [Server]
        private async void ResetAlley()
        {
            //Marks as resetting, so this method only gets called once
            _isResetting = true;
            
            //Begin reset animation
            //SetTrigger() needs to be called on NetworkAnimator
            GetComponent<NetworkAnimator>().SetTrigger("Reset Arm");

            //Await x milliseconds for animations to play
            await Task.Delay((int)(animationDelaySeconds * 1000));

            //Destroy all pins 
            foreach (GameObject obj in _trackedPins.Concat(_trackedBalls))
            {
                //NetworkServer.Destroy() since they are network objects
                NetworkServer.Destroy(obj);
            }
            
            //Clear all tracked objects lists
            _trackedPins.Clear();
            _trackedBalls.Clear();

            //Spawn new objects
            SpawnPins();
            SpawnBalls();
            
            //Reset is now done, unmark isResetting
            _isResetting = false;
        }

        private void SpawnPins()
        {
            //Call spawn code for each defined spawn
            foreach (Transform spawn in pinSpawns)
            {
                //Spawn new pin, matching spawn transform position & rotation
                GameObject obj = Instantiate(pinPrefab, spawn.position, spawn.rotation);
                
                //Spawn object on network
                NetworkServer.Spawn(obj);

                //Track spawned objects so we can destroy them later
                _trackedPins.Add(obj);
            }
        }

        private void SpawnBalls()
        {
            //Call spawn code for each defined spawn
            foreach (Transform spawn in ballSpawns)
            {
                //Spawn new balls
                GameObject obj = Instantiate(ballPrefab, spawn.position, spawn.rotation);
                //Spawn object on network
                NetworkServer.Spawn(obj);

                //Track spawned objects so we can destroy them later
                _trackedBalls.Add(obj);
            }
        }
    }
}