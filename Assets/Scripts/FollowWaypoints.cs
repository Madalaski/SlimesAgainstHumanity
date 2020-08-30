using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWaypoints : MonoBehaviour
{
    [SerializeField] Transform[] route;
    int currentRouteIndex = 0;
    float timeToTravel = 3f;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = route[currentRouteIndex].position;
        StartCoroutine(TravelToNextWaypoint());
    }

    IEnumerator TravelToNextWaypoint()
    {
        float time = 0f;
        while (time < timeToTravel) {
            if (currentRouteIndex != route.Length)
            {
                transform.Translate(Time.deltaTime * (route[currentRouteIndex + 1].position - route[currentRouteIndex].position) / timeToTravel);
                time += Time.deltaTime;
                yield return null;
            }
            else
            {
                transform.Translate(Time.deltaTime * (route[0].position - route[currentRouteIndex].position) / timeToTravel);
                time += Time.deltaTime;
                yield return null;
            }
            

            
        }
        currentRouteIndex++;
        StartCoroutine(TravelToNextWaypoint());

    }
}
