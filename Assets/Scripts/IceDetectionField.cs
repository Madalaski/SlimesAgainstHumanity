using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDetectionField : MonoBehaviour
{
    bool isPlayerInside = false;

    PlayerHandler player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            isPlayerInside = true;
            if (player == null) {
                player = other.GetComponent<PlayerHandler>();
            }
        }
        
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") {
            isPlayerInside = false;
        }
    }

    public void Freeze(float freezeTime)
    {
        if (isPlayerInside)
        {
            Debug.Log("Freezing");
            
            player.FreezePlayer(0.4f*Time.deltaTime);
        }
    }
}
