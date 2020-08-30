using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindDetectionField : MonoBehaviour
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
        if (other.tag == "Player")
        {
            isPlayerInside = true;
            if (player == null) {
                player = other.GetComponent<PlayerHandler>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            isPlayerInside = false;
        }
    }

    public void WindKnockback(float knockBackForce, float knockUpForce)
    {
        if (isPlayerInside)
        {
            Debug.Log("Wind knock back");
            player.PushPlayer((transform.forward + Vector3.up)*20f);
        }

    }
}
