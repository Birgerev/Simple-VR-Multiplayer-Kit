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
        public Transform[] pinSpawns;
        public GameObject pinPrefab;
        [Space] 
        public Transform[] ballSpawns;
        public GameObject ballPrefab;
        [Space] 
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
            
            if(AllBallsHaveReachedTrigger() && !_isResetting)
                ResetAlley();
        }

        private bool AllBallsHaveReachedTrigger()
        {
            // Iterate through each tracked ball
            foreach (GameObject ball in _trackedBalls)
            {
                // Check if the ball's position is not within the ballBounds
                if (!ballResetTrigger.bounds.Contains(ball.transform.position))
                    return false; // Return false if any ball is outside the bounds
            }
            
            //If all tracked balls were inside, return true
            return true;
        }


        [Server]
        private async void ResetAlley()
        {
            _isResetting = true;
            GetComponent<NetworkAnimator>().SetTrigger("Reset Arm");

            //Await milliseconds for animations
            await Task.Delay((int)(animationDelaySeconds * 1000));

            //Clear old objects
            foreach (GameObject obj in _trackedPins.Concat(_trackedBalls))
            {
                NetworkServer.Destroy(obj);
            }
            _trackedPins.Clear();
            _trackedBalls.Clear();

            //Pin Spawning
            foreach (Transform spawn in pinSpawns)
            {
                //Spawn new pins
                GameObject obj = Instantiate(pinPrefab, spawn.position, spawn.rotation);
                //Spawn object on network
                NetworkServer.Spawn(obj);

                //Track spawned objects so we can destroy them later
                _trackedPins.Add(obj);
            }

            //Ball Spawning
            foreach (Transform spawn in ballSpawns)
            {
                //Spawn new balls
                GameObject obj = Instantiate(ballPrefab, spawn.position, spawn.rotation);
                //Spawn object on network
                NetworkServer.Spawn(obj);

                //Track spawned objects so we can destroy them later
                _trackedBalls.Add(obj);
            }
            _isResetting = false;
        }
    }
}