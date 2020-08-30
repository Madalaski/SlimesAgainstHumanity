using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireSlimeBehaviour : MonoBehaviour
{
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] GameObject center;

    public ParticleSystem particleSystem;
    GameObject player;
    float fireRate = 2f;
    bool inCoroutine = false;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] fireChargeClips;
    [SerializeField] AudioClip[] fireShootClips;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FireAttackBehaviour()
    {
        if (!inCoroutine)
        {
            StartCoroutine(Fireball());
            inCoroutine = true;
        }   
    }

    public void StopFireAttack()
    {
        StopCoroutine(Fireball());
        inCoroutine = false;
    }

    IEnumerator Fireball()
    {
        float time = 0f;
        bool playedSound = false;
        while(time < fireRate)
        {
            time += Time.deltaTime;
            if(playedSound==false && time / fireRate > 0.5f)
            {
                audioSource.PlayOneShot(fireChargeClips[Random.Range(0, fireChargeClips.Length)]);
                playedSound = true;
            }
            yield return null;
        }
        particleSystem.Play();
        ShootFireBallToTarget();
        StartCoroutine(Fireball());
    }

    private void ShootFireBallToTarget()
    {
        audioSource.PlayOneShot(fireShootClips[Random.Range(0, fireShootClips.Length)]);
        GameObject fireball = Instantiate(fireballPrefab, center.transform.position, center.transform.rotation);
       
        Rigidbody rigidbody = fireball.GetComponent<Rigidbody>();

        
        Vector3 vectorToTarget = player.transform.position - center.transform.position;
        vectorToTarget.y = 0;
        float distance = vectorToTarget.magnitude;

        float force = Mathf.Sqrt(-Physics.gravity.y / (Mathf.Sqrt(2) * (distance - vectorToTarget.y)));

        vectorToTarget = Quaternion.AngleAxis(20f, Vector3.Cross(Vector3.up, vectorToTarget)) * vectorToTarget * force * 1.5f;
        vectorToTarget = new Vector3(vectorToTarget.x, -vectorToTarget.y, vectorToTarget.z);
        rigidbody.velocity = vectorToTarget;
    }
}
