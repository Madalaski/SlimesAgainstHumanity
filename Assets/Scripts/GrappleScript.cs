using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GrappleState {
    RELEASED,
    FIRING,
    REELING,
    HOOKED,
    COOLDOWN
}

public class GrappleScript : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip firingSound;

    [SerializeField] AudioClip[] windNoises;
    [SerializeField] AudioClip[] earthNoises;
    [SerializeField] AudioClip[] fireNoises;
    [SerializeField] AudioClip iceNoise;
    float ammo = 0f;

    float timeLastCooldown = 0f;
    float coolDownTime = 1f;

    public GrappleState state = GrappleState.RELEASED;

    public Transform grappleHook;

    public float hookDistance = 50f;

    public float hookTime = 2f;

    Transform currentTarget = null;

    public LayerMask slimeLayer;

    RaycastHit targetDetect;

    bool isTarget;

    GunScript gun;

    HookScript hook;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = iceNoise;
        gun = GetComponentInChildren<GunScript>();
        hook = grappleHook.GetComponent<HookScript>();  
        hook.ammo = 1f; 
    }

    // Update is called once per frame
    void Update()
    {

        if (state == GrappleState.COOLDOWN && Time.time > timeLastCooldown + coolDownTime) {
            state = GrappleState.RELEASED;
            hook.ammo = 1f;
            hook.cooldown = false;
        }

        if (state == GrappleState.RELEASED && hook.hooked) {
            state = GrappleState.HOOKED;
            hook.targetHook = 1f;
            ammo = 1f;
            gun.UpdateAmmo(ammo, hook.capturedType);
            hook.ammo = ammo;
        }

        if (state != GrappleState.REELING) {
            gun.LookInDirection((currentTarget == null || state == GrappleState.HOOKED) ? transform.forward : currentTarget.position - gun.transform.position);
        }
        
        if (currentTarget == null) {
            isTarget = Physics.BoxCast(transform.position, new Vector3(1.5f, 1.5f, 1.5f), transform.forward, out targetDetect, transform.rotation, 100f, slimeLayer);
            if (isTarget) {
                currentTarget = targetDetect.transform;
            }
            
        }
        else {

            /*if ((currentTarget.position - transform.position).sqrMagnitude < 4f && state == GrappleState.RELEASED && !hook.hooked) {
                state = GrappleState.HOOKED;
                hook.CaptureObject(currentTarget.parent.parent);
            }*/


            if (Vector3.Angle(currentTarget.position - transform.position, transform.forward) > 45f) {
                currentTarget = null;
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            if (state == GrappleState.RELEASED) {
                StartCoroutine(Hook());
                audioSource.PlayOneShot(firingSound);
            }
            else {
                
        
            }
        }

        if (state == GrappleState.HOOKED) {
            if (Input.GetMouseButtonDown(0) && hook.capturedType != SlimeType.Ice) {
                gun.Fire();
                if (state == GrappleState.HOOKED) {
                    hook.UseSlime();
                    switch (hook.capturedType) {
                        case SlimeType.Wind:
                            audioSource.PlayOneShot(windNoises[Random.Range(0, windNoises.Length)],0.4f);
                            ammo -= 0.26f;
                            break;
                        case SlimeType.Earth:
                            audioSource.PlayOneShot(earthNoises[Random.Range(0, earthNoises.Length)]);
                            ammo -= 1.01f;
                            break;
                        case SlimeType.Fire:
                            audioSource.PlayOneShot(fireNoises[Random.Range(0, fireNoises.Length)],0.8f);
                            ammo -= 0.34f;
                            break;
                    }
                    gun.UpdateAmmo(ammo, hook.capturedType);
                    hook.ammo = ammo;
                    
                }
                
            }
            else if (hook.capturedType == SlimeType.Ice) {
                
                if (Input.GetMouseButton(0)) {

                    gun.IncreaseSpeed();
                    hook.UseSlime();
                    ammo -= 0.5f*Time.deltaTime;
                    gun.UpdateAmmo(ammo, hook.capturedType);
                    hook.ammo = ammo;
                }
                else if (Input.GetMouseButtonUp(0)) {
                    hook.PauseIceBeam();
                }
            }
        }

        

        if (ammo < 0f) {
            ammo = 0f;
            gun.UpdateAmmo(ammo, hook.capturedType);
            hook.ammo = 0f;
            hook.DestroySlime();
            hook.cooldown = true;
            state = GrappleState.COOLDOWN;
            timeLastCooldown = Time.time;
        }
    }

    IEnumerator Hook() {
        state = GrappleState.FIRING;

        float startTime = Time.time;
        gun.showLine = true;

        while(Time.time < startTime + hookTime && !hook.hooked && !hook.deflect) {
            grappleHook.localPosition = hook.initialOffset + Vector3.Lerp(Vector3.zero, Vector3.forward*hookDistance, (Time.time - startTime) / hookTime);
            yield return null;
        }

        state = GrappleState.REELING;

        hook.deflect = false;

        Vector3 gunForward = gun.transform.forward;
        float oldAngle = transform.eulerAngles.y;

        Vector3 endPoint = grappleHook.localPosition - hook.initialOffset;

        float newTime = (endPoint.z / hookDistance) * hookTime;

        startTime = Time.time;

        while(Time.time < startTime + newTime) {
            grappleHook.localPosition = hook.initialOffset + Vector3.Lerp(endPoint, Vector3.zero, (Time.time - startTime) / newTime);
            gun.LookInDirection(Quaternion.AngleAxis(transform.eulerAngles.y - oldAngle, Vector3.up) * gunForward);
            yield return null;
        }

        grappleHook.localPosition = hook.initialOffset;

        if (hook.hooked) {
            state = GrappleState.HOOKED;
            hook.targetHook = 1f;
            ammo = 1f;
            gun.UpdateAmmo(ammo, hook.capturedType);
        }
        else {
            state = GrappleState.RELEASED;
        }

        hook.deflect = false;

        gun.showLine = false;

        yield return null;
    }
}
