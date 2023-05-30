using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(NetworkAnimator))]
public class DemoBowlingAlley : NetworkBehaviour
{
    public Transform[] pinSpawns;
    public GameObject pinPrefab;
    [Space]
    public Transform[] ballSpawns;
    public GameObject ballPrefab;
    [Space] 
    public float animationDelaySeconds;
    
    private readonly List<GameObject> _trackedObjects = new();

    private void Start()
    {
        if(isServer)
            ResetAlley();
    }

    [Server]
    public async void ResetAlleyButton()
    {
        GetComponent<NetworkAnimator>().SetTrigger("Reset Arm");
        
        //Await milliseconds for animations
        await Task.Delay((int)(animationDelaySeconds*1000));

        ResetAlley();
    }
    
    
    [Server]
    private void ResetAlley()
    {
        //Clear old objects
        foreach (GameObject obj in _trackedObjects)
        {
            NetworkServer.Destroy(obj);
        }

        //Spawn new pins
        foreach (Transform spawn in pinSpawns)
        {
            GameObject obj = Instantiate(pinPrefab, spawn.position, spawn.rotation);
            NetworkServer.Spawn(obj);
            
            _trackedObjects.Add(obj);
        }
        
        //Spawn new balls
        foreach (Transform spawn in ballSpawns)
        {
            GameObject obj = Instantiate(ballPrefab, spawn.position, spawn.rotation);
            NetworkServer.Spawn(obj);
            
            _trackedObjects.Add(obj);
        }
    }
}
