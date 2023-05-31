using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    private readonly List<GameObject> _trackedObjects = new();

    private void Start()
    {
        //Reset alley when game starts
        if (isServer)
            ResetAlley();
    }

    private void Update()
    {
        
    }


    [Server]
    private async void ResetAlley()
    {
        GetComponent<NetworkAnimator>().SetTrigger("Reset Arm");
        
        //Await milliseconds for animations
        await Task.Delay((int)(animationDelaySeconds*1000));
        
        //Clear old objects
        foreach (GameObject obj in _trackedObjects)
        {
            NetworkServer.Destroy(obj);
        }

        //Pin Spawning
        foreach (Transform spawn in pinSpawns)
        {
            //Spawn new pins
            GameObject obj = Instantiate(pinPrefab, spawn.position, spawn.rotation);
            //Spawn object on network
            NetworkServer.Spawn(obj);
            
            //Track spawned objects so we can destroy them later
            _trackedObjects.Add(obj);
        }
        
        //Ball Spawning
        foreach (Transform spawn in ballSpawns)
        {
            //Spawn new balls
            GameObject obj = Instantiate(ballPrefab, spawn.position, spawn.rotation);
            //Spawn object on network
            NetworkServer.Spawn(obj);
            
            //Track spawned objects so we can destroy them later
            _trackedObjects.Add(obj);
        }
    }
}
