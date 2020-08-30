using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSlimeBehaviour : MonoBehaviour
{
    GameObject player;
    [SerializeField] GameObject iceHitBox;
    public ParticleSystem particleSystem;

    public Transform iceBeam;
    public List<ParticleCollisionEvent> collisionEvents;
    float slowDownSpeed = 2f;
    float rotationSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IceAttackBehaviour()
    {
        iceBeam.gameObject.SetActive(true);
        iceBeam.Rotate(Vector3.up * Time.deltaTime*720f, Space.Self);
        particleSystem.Play();
        Quaternion playerLookRotation = Quaternion.LookRotation((player.transform.position + Vector3.up - iceHitBox.transform.position).normalized);
        iceHitBox.transform.rotation = Quaternion.Lerp(iceHitBox.transform.rotation, playerLookRotation, Time.deltaTime * rotationSpeed);

        iceHitBox.GetComponent<IceDetectionField>().Freeze(slowDownSpeed);
        //iceHitBox.transform.LookAt(player.transform.position +2* Vector3.up);
        return true;
    }

    public void StopIceBehaviour()
    {
        iceBeam.gameObject.SetActive(false);
        particleSystem.Stop();
    }
}
