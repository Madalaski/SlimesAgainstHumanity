using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquelchSoundHandler : MonoBehaviour
{
    [SerializeField] SlimeMover slimeMover;
    [SerializeField] AudioSource squelchSource;
    [SerializeField] AudioClip[] squelchClips;
    [SerializeField] AudioClip[] randomNoises;

    float squelchCooldown = 1f;
    bool readyToSquelch = false;
    bool isInAir = false;
    float randomNoiseMinTime = 6f;
    float randomNoiseMaxTime = 12f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayRandomNoise());
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void PlaySquelch(float volume)
    {
        squelchSource.PlayOneShot(squelchClips[Random.Range(0,squelchClips.Length)], volume);
    }

    IEnumerator PlayRandomNoise()
    {
        yield return new WaitForSeconds(Random.Range(randomNoiseMinTime, randomNoiseMaxTime));
        squelchSource.PlayOneShot(randomNoises[Random.Range(0, randomNoises.Length)]);
        StartCoroutine(PlayRandomNoise());
    }

}
