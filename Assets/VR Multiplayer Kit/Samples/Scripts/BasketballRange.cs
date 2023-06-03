using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace VRMultiplayerStarterKit.Samples
{
    [RequireComponent(typeof(BoxCollider))]
    public class BasketballRange : NetworkBehaviour
    {
        public Text scoreText;
        [Space]
        [SyncVar] public int score;
    

        // Update is called once per frame
        void Update()
        {
            scoreText.text = score.ToString();
        }

        private void OnTriggerEnter(Collider other)
        {
            //When object enters hoop
            score++;
        }
    }

}