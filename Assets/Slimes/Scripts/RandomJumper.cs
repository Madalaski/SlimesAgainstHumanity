using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomJumper : MonoBehaviour
{
    float maxForce = 150f;
    float maxXZMovement = 3f;
    float maxYMovement = 4f;


    public Rigidbody[] bones;
    public GameObject center;

    Vector3 randomTarget;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BounceAfterCooldown());
    }

    // Update is called once per frame
    void Update()
    {
        
        GenerateRandomTarget();
        for (int i = 0; i < bones.Length; i++)
        {
            Vector3 vertexDirection = (center.transform.position + randomTarget) - bones[i].transform.position;
            bones[i].AddForce(vertexDirection * Random.Range(maxForce / 2, maxForce));
        }
        
    }

    private void GenerateRandomTarget()
    {
        randomTarget = new Vector3(Random.Range(-maxXZMovement, maxXZMovement), Random.Range(0.5f, maxYMovement), Random.Range(-maxXZMovement, maxXZMovement) );

    }

    IEnumerator BounceAfterCooldown()
    {
        float timeToWait = Random.Range(3f, 5f);
        yield return new WaitForSeconds(timeToWait);
        GenerateRandomTarget();
        for (int i = 0; i < bones.Length; i++)
        {
            Vector3 vertexDirection = (center.transform.position + randomTarget) - bones[i].transform.position;
            bones[i].AddForce(vertexDirection * Random.Range(maxForce / 2, maxForce));
        }
        StartCoroutine(BounceAfterCooldown());
    }
}
