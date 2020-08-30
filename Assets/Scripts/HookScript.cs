using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookScript : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip[] captureSounds;
    [SerializeField] AudioClip deflectSound;
    [SerializeField] AudioClip lockSound; 

    public Transform dummySlime;

    SkinnedMeshRenderer slimeMesh;
    MeshRenderer innerSlimeMesh;

    
    public float targetHook = 0f;
    public float ammo = 1f;

    [Header("Slime Forces")]
    public float windForce = 20f;

    public float fireForce = 20f;

    public float iceForce = 30f;
    
    public float earthForce = 15f;

    public LayerMask slimeLayer;

    public bool deflect = false;
    public Vector3 deflectDir;

    public bool hooked = false;

    public bool cooldown = false;

    public Vector3 initialOffset;

    public Transform fireballPrefab;

    public ParticleSystem[] effects;

    public Transform iceBeam;

    SkinnedMeshRenderer skinnedMesh;

    public SlimeType capturedType;
    Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        slimeMesh = dummySlime.GetComponentInChildren<SkinnedMeshRenderer>();
        innerSlimeMesh = dummySlime.GetComponentInChildren<MeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>();
        body = GetComponent<Rigidbody>();
        initialOffset = transform.localPosition;

        
    }

    void Update() {
        skinnedMesh.SetBlendShapeWeight(0, Mathf.Lerp(skinnedMesh.GetBlendShapeWeight(0), (1f-ammo)*100f, Time.deltaTime*10f));
        skinnedMesh.SetBlendShapeWeight(1, Mathf.Lerp(skinnedMesh.GetBlendShapeWeight(1), targetHook*100f, Time.deltaTime*10f));
        dummySlime.localScale = Vector3.one * Mathf.Lerp(dummySlime.localScale.x, 0.4f - (0.2f*(1f-ammo)), Time.deltaTime*10f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.tag == "Slime") {
            if (!hooked && !cooldown) {
                audioSource.PlayOneShot(lockSound);
                audioSource.PlayOneShot(captureSounds[Random.Range(0, captureSounds.Length)]);
                CaptureObject(other.transform.parent.parent);
            }
        }
        else if (other.transform.tag == "Ground") {
            if (!hooked && transform.parent.parent.GetComponent<GrappleScript>().state == GrappleState.FIRING)
            {
                audioSource.PlayOneShot(deflectSound);
            }
            
            deflect = true;
            deflectDir = transform.parent.parent.parent.InverseTransformDirection(transform.forward);
        }
    }

    public void CaptureObject(Transform obj) {
        Material[] innerMats = innerSlimeMesh.materials;
        innerMats[0] = obj.GetComponentInParent<SlimeStateManager>().innerSlimeMaterial;
        slimeMesh.material = obj.GetComponentInChildren<SkinnedMeshRenderer>().material;
        capturedType = obj.GetComponentInParent<SlimeStateManager>().CaptureSlime();
        
        innerSlimeMesh.materials = innerMats;
        dummySlime.gameObject.SetActive(true);
        hooked = true;
    }

    public void UseSlime() {
        effects[(int)capturedType].Play();

        Collider[] slimes;
        switch (capturedType) {
            case SlimeType.Wind:
                slimes = Physics.OverlapSphere(transform.position, 10f, slimeLayer, QueryTriggerInteraction.Ignore);
                for (int i = 0; i < slimes.Length; i++) {
                    Vector3 dir = slimes[i].transform.position - transform.parent.parent.position;
                    dir.y = 0f;
                    if (Vector3.Angle(dir, transform.parent.parent.parent.forward) < 45f) {
                        slimes[i].GetComponentInParent<SlimeMover>().SetSlimeVelocity(dir.normalized*windForce);
                    }
                }
                break;

            case SlimeType.Fire:
                Rigidbody rigidbody = Instantiate(fireballPrefab, transform.position + -transform.right*1.3f, transform.rotation).GetComponent<Rigidbody>();
                rigidbody.GetComponent<FireballDetectionField>().enemy = false;

                rigidbody.velocity = -transform.right * fireForce + transform.parent.parent.parent.GetComponent<PlayerHandler>().currentVelocity;
                break;

            case SlimeType.Earth:
                slimes = Physics.OverlapSphere(transform.position, 10f, slimeLayer, QueryTriggerInteraction.Ignore);
                for (int i = 0; i < slimes.Length; i++) {
                    Vector3 dir = slimes[i].transform.position - transform.parent.parent.position;
                    dir.y = 0f;         
                    slimes[i].GetComponentInParent<SlimeMover>().SetSlimeVelocity(Vector3.up*earthForce);
                }
                break;
            
            case SlimeType.Ice:
                RaycastHit hitInfo;
                
                float range;
                if (Physics.Raycast(transform.position, -transform.right, out hitInfo, 100f, ~slimeLayer, QueryTriggerInteraction.Ignore)) {
                    range = hitInfo.distance;
                }
                else {
                    range = 100f;
                }

                slimes = Physics.OverlapCapsule(transform.position, transform.position + -transform.right*range, 0.5f, slimeLayer, QueryTriggerInteraction.Ignore);
                iceBeam.localPosition = Vector3.left*(0.483f + range);
                iceBeam.localScale = new Vector3(iceBeam.localScale.x, range, iceBeam.localScale.z);
                iceBeam.gameObject.SetActive(true);

                for (int i = 0; i < slimes.Length; i++) {      
                    slimes[i].GetComponentInParent<SlimeMover>().Freeze(3f);
                }

                break;
            
        }
    }

    public void PauseIceBeam() {
        effects[(int)capturedType].Stop();
        iceBeam.gameObject.SetActive(false);
    }

    public void DestroySlime() {
        if (capturedType == SlimeType.Ice) PauseIceBeam();
        dummySlime.gameObject.SetActive(false);
        targetHook = 0f;
        hooked = false;
    }
}
