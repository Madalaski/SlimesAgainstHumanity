using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    bool inSubMenu = false;

    [SerializeField] GameObject aboutTab;
    [SerializeField] RawImage fadeScreen;

    float fadeTime = 2f;

    Color bufferColor;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        aboutTab.SetActive(false);
        StartCoroutine(FadeToWhite());
    }

    Color SetAlpha(Color color, float alpha)
    {
        bufferColor = color;
        bufferColor.a = alpha;
        return bufferColor;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        Debug.Log("Play Game");
        StartCoroutine(FadeToBlackAndPlay());
        
        
    }

    IEnumerator FadeToBlackAndPlay()
    {
        BackgroundTexture.thisBackground.StartRendering();
        fadeScreen.gameObject.SetActive(true);
        MusicManager.current.StartCrossFade();
        float time = 0;
        while(time < fadeTime)
        {
            time += Time.deltaTime;
            yield return null;
            fadeScreen.color = SetAlpha(fadeScreen.color, time / fadeTime);
        }
        
        SceneManager.LoadScene(1);
    }

    IEnumerator FadeToWhite()
    {
        yield return new WaitUntil(() => BackgroundTexture.thisBackground != null);
        BackgroundTexture.thisBackground.StartRendering();
        fadeScreen.gameObject.SetActive(true);
        float time = 0;
        fadeScreen.color = SetAlpha(fadeScreen.color, 1);
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            yield return null;
            fadeScreen.color = SetAlpha(fadeScreen.color, 1-(time / fadeTime));
        }
        BackgroundTexture.thisBackground.StopRendering();
        fadeScreen.gameObject.SetActive(false);
    }


    public void PlayEndless()
    {
        Debug.Log("Play Endless");
    }
    
    public void ActiveSettings()
    {
        if (!inSubMenu)
        {
            inSubMenu = true;
            Debug.Log("Activate Settings");
        }
        
    }

    public void ExitSettings()
    {
        if (inSubMenu)
        {
            inSubMenu = false;

            Debug.Log("Exit Settings");
        }
        
    }

    public void ActivateAbout()
    {
        if (!inSubMenu)
        {
            aboutTab.SetActive(true);
            inSubMenu = true;
            Debug.Log("Activate About");
        }
        
    }

    public void ExitAbout()
    {
        if (inSubMenu)
        {
            aboutTab.SetActive(false);
            inSubMenu = false;
            Debug.Log("Exit About");
        }
        
    }
}
