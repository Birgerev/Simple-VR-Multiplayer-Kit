using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class XRMultiplayerInteractor : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
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

    [Client]
    private void InteractorSelectEvent(SelectEnterEventArgs args)
    {
        //Try to find a network identity component on interactable object or its parents
        NetworkIdentity networkIdentity = args.interactableObject.transform.GetComponentInParent<NetworkIdentity>();
        
        if(networkIdentity == null)
            return;
        
        //If netId is found, claim authority of it
        ClaimAuthorityOfIdentity(networkIdentity);
    }

    [Command]
    private void ClaimAuthorityOfIdentity(NetworkIdentity identityToClaim)
    {
        //Reclaim any authority from previous clients
        identityToClaim.RemoveClientAuthority();
        //Give authority to client who owns the player
        identityToClaim.AssignClientAuthority(connectionToClient);
    }
}
