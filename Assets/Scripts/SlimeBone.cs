using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBone : MonoBehaviour
{
    public bool isTouchingGround = false;
    [SerializeField] SquelchSoundHandler squelchHandler;
    float impulseForVolume = 8f;
    float minVolume = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground")
        {
            isTouchingGround = true;
            float volumeScale = minVolume + collision.impulse.magnitude / impulseForVolume;
            squelchHandler.PlaySquelch(volumeScale);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            isTouchingGround = false;
        }
    }
}
