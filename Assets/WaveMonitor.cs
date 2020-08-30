using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveMonitor : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    float hoverPeriod = 2f;
    float hoverOffset = 10f;
    bool isUp = true;

    float propellerRotationSpeed = 120f;
    [SerializeField] Transform[] propellers;

    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPos + Vector3.up * 0.5f * Mathf.Sin(Time.time * (2f * Mathf.PI / hoverPeriod)) * hoverOffset;
        foreach(Transform propeller in propellers)
        {
            propeller.Rotate(Vector3.forward, propellerRotationSpeed * Time.deltaTime);
        }
    }

    public void UpdateWaveLabel(int waveNumber)
    {
        text.text = "Wave " + (waveNumber +1);
    }
}
