using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheFirstPerson;

public class PlayerHandler : TFPExtension
{
    // Start is called before the first frame update

    Vector3 pushDirection = Vector3.zero;

    bool beenPushed = false;

    float freezePercentage = 0f;
    float freezeAmount = 0f;

    bool frozen = false;

    float control = 1f;

    float timeSincePushed = 0f;

    Vector3 floatingPlatform;

    public Vector3 currentVelocity;

    public bool grounded;

    AudioSource audioSource;
    [SerializeField] AudioClip[] footsteps;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landingSound;

    float strideLength = 6f;
    float maxRunSpeed = 20f;
    float maxStepVolume = 0.7f;

    bool isInAir = false;
    float previousYvelocity = 0f;
    float maxFallLimit = 9f;

    Coroutine currentFootsteps;
    bool footstepPlaying;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentFootsteps = StartCoroutine(Footsteps());
        footstepPlaying = true;
    }


    IEnumerator Footsteps()
    {

        while (true) {
            if (currentVelocity.magnitude < strideLength)
            {
                yield return new WaitForFixedUpdate();
            }
            else
            {
                if (grounded)
                {
                    audioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)], maxStepVolume * Mathf.Clamp01(currentVelocity.magnitude / maxRunSpeed));
                }
                yield return new WaitForSeconds(strideLength / currentVelocity.magnitude);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isInAir && grounded)
        {
            audioSource.PlayOneShot(landingSound, Mathf.Clamp01(-previousYvelocity / maxFallLimit));
            if (!footstepPlaying) {
                currentFootsteps = StartCoroutine(Footsteps());
                footstepPlaying = true;
            }
            isInAir = false;
        }
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            audioSource.PlayOneShot(jumpSound);
            StopCoroutine(currentFootsteps);
            footstepPlaying = false;
            isInAir = true;
        }

        
        if (freezeAmount > 0f) {
            freezeAmount -= 0.2f*Time.deltaTime;
        }
        if (frozen) {
            freezePercentage = 1f;
            if (freezeAmount < 0f) {
                frozen = false;
                freezePercentage = 0f;
            }
        }
        else {
            freezePercentage = freezeAmount;
            if (freezeAmount > 1f) {
                frozen = true;
                freezePercentage = 1f;
            }
        }
        
        isInAir = !grounded;
        if (isInAir)
        {
            previousYvelocity = currentVelocity.y;
        }

        GameController.current.frozen = freezePercentage;
    }

    public void PushPlayer (Vector3 direction) {
        beenPushed = true;
        pushDirection += direction;
    }

    public override void ExPreUpdate(ref TFPData data, TFPInfo info) {
        if (control == 0f && Time.time > timeSincePushed + 0.3f && data.grounded) {
            control = 1f;
            data.moving = true;
            print("Regain control");
        }
        else if (control == 0f) {
            data.moving = false;
        }
        data.xIn = Mathf.Lerp(data.xIn, 0f, freezePercentage);
        data.yIn = Mathf.Lerp(data.yIn, 0f, freezePercentage);
    }

    public override void ExPreMove(ref TFPData data, TFPInfo info) {
        data.currentMove += floatingPlatform;
        floatingPlatform = Vector3.zero;
        if (beenPushed) {
            data.yVel += pushDirection.y;
            data.currentMove += new Vector3(pushDirection.x, 0f, pushDirection.z);
            
            timeSincePushed = Time.time;
            pushDirection = Vector3.zero;
            control = 0f;
            beenPushed = false;
        }
    }

    public override void ExPostMove(ref TFPData data, TFPInfo info) {
        currentVelocity = data.moveDelta / Time.deltaTime;
        grounded = data.grounded;
    }

    public void FreezePlayer (float amount) {
        if (!frozen) {
            freezeAmount += amount;
        }
    }

    public void FloatingPlatforms(Vector3 velocity) {
        floatingPlatform = velocity * Time.deltaTime;
    }
}
