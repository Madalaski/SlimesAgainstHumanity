using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSlimeSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] slimePrefabs;
    float xBound = 10f;
    float yBound = 6f;
    float minSpawnRate = 3f;
    float maxSpawnRate = 6f;
    int maxSlimeCount = 5;


    int currentSlimeCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnMenuSlimes());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnMenuSlimes()
    {
        if (currentSlimeCount < maxSlimeCount)
        {
            GameObject slime = Instantiate(slimePrefabs[Random.Range(0, slimePrefabs.Length)], transform.position + new Vector3(Random.Range(-xBound, xBound), 0f, Random.Range(-yBound, yBound)), transform.rotation);
            currentSlimeCount++;
        }

        float time = 0f;
        float randomSpawnRate = Random.Range(minSpawnRate, maxSpawnRate);
        while (time < randomSpawnRate)
        {
            time += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(SpawnMenuSlimes());
        
    }
}


