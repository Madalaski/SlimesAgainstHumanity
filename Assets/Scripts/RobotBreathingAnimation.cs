using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBreathingAnimation : MonoBehaviour
{
    [SerializeField] GameObject robotHead;

    float timePerBreath = 1f;

    float maxOffset = 0.1952f;
    float minOffset = 0.1844f;
    bool isBreathingIn = true;
    SkinnedMeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        StartCoroutine(Breath());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Breath()
    {
        float time = 0f;
        if (isBreathingIn)
        {
            while (time < timePerBreath)
            {
                time += Time.deltaTime;
                meshRenderer.SetBlendShapeWeight(0, 100*(1 - time / timePerBreath));
                robotHead.transform.Translate(new Vector3(0, (maxOffset - minOffset) * 100f / timePerBreath * Time.deltaTime), 0);
                yield return null;
            }
        }
        else
        {
            while (time < timePerBreath)
            {
                time += Time.deltaTime;
                meshRenderer.SetBlendShapeWeight(0, 100*(time / timePerBreath));
                robotHead.transform.Translate(new Vector3(0, -(maxOffset - minOffset) * 100f / timePerBreath * Time.deltaTime), 0);
                yield return null;
            }
        }
        isBreathingIn = !isBreathingIn;
        StartCoroutine(Breath());
        
    }
}
