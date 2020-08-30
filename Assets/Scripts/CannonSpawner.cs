using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CannonSpawner : MonoBehaviour
{
    [SerializeField] GameObject windSlimePrefab;
    [SerializeField] GameObject fireSlimePrefab;
    [SerializeField] GameObject iceSlimePrefab;
    [SerializeField] GameObject earthSlimePrefab;

    [SerializeField] GameObject cannonBase;
    [SerializeField] GameObject cannonBarrel;

    LayerMask groundLayer;
    float cannonAdjustmentTime = 1f;
    float cannonFireWaitTime = 0.1f;
    float barrelOffset = 13.84f;

    AudioSource audioSource;
    [SerializeField] AudioClip clickClip;
    [SerializeField] AudioClip fireClip;

    bool isBusy = false;

    List<SlimeType> slimesToFire = new List<SlimeType>();

    [SerializeField] Transform[] corners;    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        groundLayer = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        if (slimesToFire.Count != 0 && !isBusy)
        {
            FireSlimeInPositionOne();
        }
    }

    private void FireSlimeInPositionOne()
    {
        GameObject slimeToFire;
        switch (slimesToFire[0])
        {
            case SlimeType.Wind:
                slimeToFire = windSlimePrefab;
                break;
            case SlimeType.Ice:
                slimeToFire = iceSlimePrefab;
                break;
            case SlimeType.Fire:
                slimeToFire = fireSlimePrefab;
                break;
            case SlimeType.Earth:
                slimeToFire = earthSlimePrefab;
                break;
            default:
                slimeToFire = windSlimePrefab;
                break;
        }
        StartCoroutine(FireSlime(slimeToFire));
        slimesToFire.RemoveAt(0);
    }

    Vector3 GenerateRandomTarget()
    {
        Vector2 randomPoint = Random.insideUnitCircle*100f;
        RaycastHit hit;
        int i = 0;
        while (!Physics.Raycast(transform.position + new Vector3(randomPoint.x, 5f, randomPoint.y), Vector3.down, out hit, 100f, groundLayer) && i < 100) {
            randomPoint = Random.insideUnitCircle*100f;
            i++;
        }

        if (i == 100) {
            return Vector3.zero;
        }

        return hit.point;
        /*Vector3 firstVector = corners[0].position + UnityEngine.Random.Range(0f, 1f) * corners[1].position;
        Vector3 secondVector = corners[2].position + UnityEngine.Random.Range(0f, 1f) * corners[3].position;
        return firstVector + UnityEngine.Random.Range(0f, 1f) * secondVector;*/
    }

    void AddRandomSlimeToQueue()
    {
        SlimeType slimeType;
        float random = UnityEngine.Random.Range(0f, 1f);
        if (random < 0.25)
        {
            slimeType = SlimeType.Wind;
        }
        else if (random < 0.5f)
        {
            slimeType = SlimeType.Fire;
        }
        else if(random < 0.75f)
        {
            slimeType = SlimeType.Wind;
        }
        else
        {
            slimeType = SlimeType.Earth;
        }
        slimesToFire.Add(slimeType);
    }

    IEnumerator FireSlime(GameObject slimePrefab)
    {
        
        isBusy = true;
        float time = 0;
        float randomWaitTime = Random.Range(0f, 0.4f);
        while (time < randomWaitTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        audioSource.Play();
        time = 0;
        float fireAngle = UnityEngine.Random.Range(330f,360f);
        float initialAngle = cannonBarrel.transform.localEulerAngles.x;
        Vector3 fireDestination = GenerateRandomTarget();
        float angleToDestination = Vector3.SignedAngle(cannonBase.transform.forward, fireDestination - cannonBase.transform.position, Vector3.up);
        
        while (time < cannonAdjustmentTime)
        {
            cannonBarrel.transform.localEulerAngles = new Vector3(Mathf.Lerp(initialAngle, fireAngle, time / cannonAdjustmentTime), 0, 0);
            cannonBase.transform.Rotate(Vector3.up, Time.deltaTime*angleToDestination/cannonAdjustmentTime);
            yield return null;
            time += Time.deltaTime;
        }
        audioSource.Stop();
        audioSource.PlayOneShot(clickClip);
        time = 0;
        while (time < cannonFireWaitTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        audioSource.PlayOneShot(fireClip);
        Vector3 cannonForceRequired = CalculateParabolaForce(fireDestination, 60f);
        GameObject firedSlime = Instantiate(slimePrefab, cannonBarrel.transform.position + cannonBarrel.transform.forward.normalized * barrelOffset, cannonBarrel.transform.rotation);
        firedSlime.GetComponentInChildren<SlimeMover>().SetSlimeVelocity(1.5f*cannonForceRequired);
        isBusy = false;
    }

    private Vector3 CalculateParabolaForce(Vector3 destination, float angle)
    {
        Vector3 spawnPoint = cannonBarrel.transform.position + cannonBarrel.transform.forward.normalized * barrelOffset;
        Vector3 vectorToTarget = destination - spawnPoint;
        float yDiff = vectorToTarget.y;
        vectorToTarget.y = 0;
        float distance = vectorToTarget.magnitude;

        float force = Mathf.Sqrt(-Physics.gravity.y / (2 * (Mathf.Sin(angle*Mathf.Deg2Rad) * distance - Mathf.Cos(angle*Mathf.Deg2Rad) * yDiff) ) );

        vectorToTarget = Quaternion.AngleAxis(-angle, Vector3.Cross(Vector3.up, vectorToTarget)) * vectorToTarget * force;
        return vectorToTarget;
    }

    public void QueueSlimesToFire(List<SlimeType> slimesToQueue)
    {
        slimesToFire.AddRange(slimesToQueue);
    }

    public void QueueSlimeToFire(SlimeType slimeToQueue)
    {
        slimesToFire.Add(slimeToQueue);
    }
}
