using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class GameController : MonoBehaviour
{

    public static GameController current;

    // Start is called before the first frame update
    [SerializeField] TextAsset waveInformation;

    CannonSpawner[] cannons;
    public float score;
    public float shownScore;

    public float additional;

    public float multiplier;
    float timeSinceLastMultiplier;
    float comboTime = 3f;

    public float scoreSpeed = 10f;

    public TMP_Text scoreText;
    public TMP_Text additionalText;
    public TMP_Text multiplierText;

    public Image radial;

    public float frozen;
    public PostProcessVolume volume;

    [SerializeField] RawImage fadeScreen;
    float fadeTime = 2.5f;

    bool allSlimesDead = false;
    float maxWaveLength = 25f;

    float slimeCheckInterval = 2f;

    int currentWave = 0;
    Wave[] waves;
    WaveMonitor[] waveMonitors;
    void Start()
    {
        
        
        StartCoroutine(FadeToWhite());
        current = this;
        waveMonitors = GameObject.FindObjectsOfType<WaveMonitor>();
        timeSinceLastMultiplier = Time.time - comboTime;
        GameObject[] cannonGameObjects = GameObject.FindGameObjectsWithTag("Cannon");
        List<CannonSpawner> cannonList = new List<CannonSpawner>();
        foreach(GameObject cannon in cannonGameObjects)
        {
            cannonList.Add(cannon.GetComponent<CannonSpawner>());
        }
        cannons = cannonList.ToArray();
        ReadWaveInformation();
        StartCoroutine(CheckSlimePopulation());
        StartCoroutine(WaveCycle());
    }

    IEnumerator CheckSlimePopulation()
    {
        yield return new WaitForSeconds(slimeCheckInterval);
        if (GameObject.FindGameObjectsWithTag("Slime").Length == 0)
        {
            allSlimesDead = true;
        }
        else
        {
            allSlimesDead = false;
        }
        StartCoroutine(CheckSlimePopulation());
    }
    IEnumerator WaveCycle()
    {
        StartWave(currentWave);
        foreach(WaveMonitor waveMonitor in waveMonitors)
        {
            waveMonitor.UpdateWaveLabel(currentWave);
        }

        allSlimesDead = false;
        float time = 0f;
        while (time < maxWaveLength && !allSlimesDead)
        {
            if (time < slimeCheckInterval)
            {
                allSlimesDead = false;
            }
            time += Time.deltaTime;

            yield return null;
        }
        if(currentWave == waves.Length-1)
        {
            while (!allSlimesDead)
            {
                yield return null;
            }
            Debug.Log("Completed");
            StartCoroutine(FadeToBlack());
            StopCoroutine(WaveCycle());
            
        }
        else
        {
            currentWave++;
            StartCoroutine(WaveCycle());
        }
        

    }

    private void ReadWaveInformation()
    {
        waves = JsonHelper.FromJson<Wave>(waveInformation.ToString());
    }

    // Update is called once per frame
    void Update()
    {

        volume.profile.GetSetting<ColorGrading>().colorFilter.value = Color.Lerp(Color.white, Color.blue, frozen);


        if (Time.time > timeSinceLastMultiplier + comboTime) {
            multiplier = 1f;
            radial.fillAmount = 1f;
        }
        else {
            radial.fillAmount = 1f - ((Time.time - timeSinceLastMultiplier) / comboTime);
        }

        additionalText.color = Color.Lerp(additionalText.color, new Color(1f, 1f, 1f, (additional > 0f) ? 1f : 0f),  0.125f);

        if (additional > 0f) {
            additional -= scoreSpeed * Time.deltaTime;
        }
        else {
            additional = 0f;
        }

        if (shownScore < score) {
            shownScore += scoreSpeed * Time.deltaTime;
        }
        else {
            shownScore = score;
        }

        scoreText.text = ((int) shownScore).ToString("D9");
        additionalText.text = string.Format("+{0}", (int)additional);
        multiplierText.text = string.Format("x{0}", (int)multiplier);
    }

    public void AddScore(float points) {
        score += points * multiplier;
        if (PlayerPrefs.HasKey("HighScore")) {
            if (score > PlayerPrefs.GetFloat("HighScore")) {
                PlayerPrefs.SetFloat("HighScore", score);
            }
        }
        additional += points * multiplier;
    }

    public void AddScore(float points, Vector3 location) {
        AddScore(points);
    }

    public void AdvanceMultiplier() {
        multiplier += 1f;
        timeSinceLastMultiplier = Time.time;
    }

    public void StartWave(int waveNumber)
    {
        Wave wave = waves[waveNumber];
        List<SlimeType> availableTypes = new List<SlimeType> { SlimeType.Wind, SlimeType.Fire, SlimeType.Earth, SlimeType.Ice };
        List<SlimeType> slimeTypesToFire = new List<SlimeType>();

        slimeTypesToFire.AddRange(Enumerable.Repeat(SlimeType.Wind, wave.windSlimeNumber));
        slimeTypesToFire.AddRange(Enumerable.Repeat(SlimeType.Ice, wave.iceSlimeNumber));
        slimeTypesToFire.AddRange(Enumerable.Repeat(SlimeType.Fire, wave.fireSlimeNumber));
        slimeTypesToFire.AddRange(Enumerable.Repeat(SlimeType.Earth, wave.earthSlimeNumber));

        while(slimeTypesToFire.Count > 0)
        {
            int indexToFire = Random.Range(0, slimeTypesToFire.Count);
            int cannonNumber = Random.Range(0, cannons.Length);
            cannons[cannonNumber].QueueSlimeToFire(slimeTypesToFire[indexToFire]);
            slimeTypesToFire.RemoveAt(indexToFire);
        }

    }

    public void PlayerDeath()
    {
        StartCoroutine(FadeToBlack());
    }
    IEnumerator FadeToWhite()
    {
        float time = 0;
        fadeScreen.color = SetAlpha(fadeScreen.color, 1);
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            yield return null;
            fadeScreen.color = SetAlpha(fadeScreen.color, 1- (time / fadeTime));
        }
        BackgroundTexture.thisBackground.StopRendering();
        fadeScreen.gameObject.SetActive(false);
    }
    IEnumerator FadeToBlack()
    {
        BackgroundTexture.thisBackground.StartRendering();
        MusicManager.current.StartCrossFade();
        fadeScreen.gameObject.SetActive(true);
        float time = 0;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            yield return null;
            fadeScreen.color = SetAlpha(fadeScreen.color, time / fadeTime);
        }
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);


    }

    Color SetAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
