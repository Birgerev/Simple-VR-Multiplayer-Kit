using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRMultiplayerStarterKit
{    
    /// <summary>
    /// Script that makes sure that all player owned interactors claim network authority
    /// over network grabbables that are grabbed
    /// </summary>
    public class XRMultiplayerInteractor : NetworkBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //Only execute on our owned player
            if (!isOwned)
                return;
            
            //Register event listeners for whenever any interactor that is part of our XR Rig grabs something
            RegisterInteractionListeners();
        }
    
        private void RegisterInteractionListeners()
        {
            //Get all player interactors in children
            foreach (var interactor in GetComponentsInChildren<XRBaseInteractor>())
            {
                //Listen for select events (when we grab objects)
                interactor.selectEntered.AddListener(InteractorSelectEvent);
            }
        }

        /// <summary>
        /// Called whenever one of the local players interactors grabs anything
        /// </summary>
        /// <param name="args">event arguments</param>
        [Client]
        private void InteractorSelectEvent(SelectEnterEventArgs args)
        {
            //Try to find a network identity component on interactable object or its parents
            NetworkIdentity grabbedObjectNetId = args.interactableObject.transform.GetComponentInParent<NetworkIdentity>();
        
            //If no net id was found, we can't claim it
            if(grabbedObjectNetId == null)
                return;
        
            //If netId is found, claim authority of it
            ClaimAuthorityOfIdentity(grabbedObjectNetId);
        }
        
        /// <summary>
        /// Client request that runs on server.
        /// Allows client to claim any network identity. (Potentially unsafe!),
        /// write safety checks here if that is important
        /// </summary>
        /// <param name="identityToClaim">The net id that the player is going to claim</param>
        [Command]
        private void ClaimAuthorityOfIdentity(NetworkIdentity identityToClaim)
        {
            //Reclaim any authority from previous clients
            identityToClaim.RemoveClientAuthority();
            //Give authority to client who owns the player
            identityToClaim.AssignClientAuthority(connectionToClient);
        }
    }
}