using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreText : MonoBehaviour
{
    TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        if (!PlayerPrefs.HasKey("HighScore")) {
            PlayerPrefs.SetFloat("HighScore", 0f);
        }
        text.text = string.Format("High Score: {0}", (int)PlayerPrefs.GetFloat("HighScore"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
