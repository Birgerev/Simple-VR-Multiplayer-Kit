using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkGrabAuthorityClaimer : NetworkBehaviour
{
    private NetworkIdentity _identity;
    
    private void Start()
    {
        //Get Identity in this object
        _identity = GetComponent<NetworkIdentity>();
        foreach (var interactable in GetComponentsInChildren<XRGrabInteractable>())
        {
            //Register event for when object is grabbed on a client
            interactable.selectEntered.AddListener(Grabbed);
        }
    }

    private void Grabbed(SelectEnterEventArgs args)
    {
        //Client sends request to server to claim authority
        RequestClaimAuthority();
    }

    [Command(requiresAuthority = false)]
    public void RequestClaimAuthority(NetworkConnectionToClient sender = null)
    {
        //Return authority to server
        _identity.RemoveClientAuthority();
        //Give authority to client who called command
        _identity.AssignClientAuthority(sender);
    }

    [Command(requiresAuthority = false)]
    public void ResignAuthority(NetworkConnectionToClient sender = null)
    {
        //Return authority to server
        _identity.RemoveClientAuthority();
    }
}
