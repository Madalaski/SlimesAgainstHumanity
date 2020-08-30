using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager current;
    [SerializeField] AudioSource menuMusic;
    [SerializeField] AudioSource battleMusic;
    float maxMenuVolume;
    float maxBattleVolume;
    float crossFadeTime = 5f;
    public bool inMenu = true;
    bool inCoroutine = false;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("MusicManager").Length > 1) {
            Destroy(this.gameObject);
        }
        else {
            current = this;
        }
        DontDestroyOnLoad(gameObject);
        maxMenuVolume = menuMusic.volume;
        maxBattleVolume = battleMusic.volume;
        if (inMenu)
        {
            battleMusic.volume = 0;
        }
        else
        {
            menuMusic.volume = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartCrossFade()
    {
        if (!inCoroutine)
        {
            inCoroutine = true;
            if (inMenu)
            {
                StartCoroutine(FadeToBattle());
            }
            else
            {
                StartCoroutine(FadeToMenu());
            }
        }
        
    }


    IEnumerator FadeToBattle()
    {
        float time = 0f;
        battleMusic.volume = 0f;
        while (time < crossFadeTime)
        {
            time += Time.deltaTime;
            battleMusic.volume = maxBattleVolume * time / crossFadeTime;
            menuMusic.volume = maxMenuVolume * (1 - time / crossFadeTime);
            yield return null;
        }
        battleMusic.volume = maxMenuVolume;
        menuMusic.volume = 0f;
        inMenu = false;
        inCoroutine = false;

    }
    
    IEnumerator FadeToMenu()
    {
        float time = 0f;
        menuMusic.volume = 0f;
        while (time < crossFadeTime)
        {
            time += Time.deltaTime;
            menuMusic.volume = maxMenuVolume * time / crossFadeTime;
            battleMusic.volume = maxBattleVolume * (1 - time / crossFadeTime);
            yield return null;
        }
        menuMusic.volume = maxMenuVolume;
        battleMusic.volume = 0f;
        inMenu = true;
        inCoroutine = false;
    }
}
