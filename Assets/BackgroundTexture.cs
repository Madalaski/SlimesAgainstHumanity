using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BackgroundTexture : MonoBehaviour
{
    public static BackgroundTexture thisBackground;
    VideoPlayer videoPlayer;
    private void Awake()
    {
        
        if (thisBackground == null)
        {
            thisBackground = this;
            videoPlayer = GetComponent<VideoPlayer>();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StopRendering()
    {
        videoPlayer.Pause();
    }

    public void StartRendering()
    {
        videoPlayer.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    
}
