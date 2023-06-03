using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace VRMultiplayerStarterKit
{
    public class OnlyAuthorityComponents : MonoBehaviour
    {
        public List<Behaviour> authorityComponents;
        private NetworkIdentity _identity;

        // Start is called before the first frame update
        void Awake()
        {
            //Get the network identity on current object or its parents
            _identity = GetComponentInParent<NetworkIdentity>();
        }

        // Update is called once per frame
        void Update()
        {
            //Every frame, enable/disable components depending on whether client has authority
            foreach (var component in authorityComponents)
            {
                component.enabled = _identity.isOwned;
            }
        }
    }
}
