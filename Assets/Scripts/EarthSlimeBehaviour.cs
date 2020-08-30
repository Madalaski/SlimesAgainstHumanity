using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthSlimeBehaviour : MonoBehaviour
{
    GameObject player;
    [SerializeField] GameObject earthHitBox;
    [SerializeField] GameObject center;
    public ParticleSystem particleSystem;
    float chillTime = 1f;
    float maxHitBoxAlpha;
    float knockUpForce = 100f;

    bool isInCoroutine = false;
    bool isAttackComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        //MeshRenderer meshRenderer = earthHitBox.GetComponent<MeshRenderer>();
        //Material material = meshRenderer.material;
        //maxHitBoxAlpha = material.color.a;
        //meshRenderer.material.color = new Color(material.color.r, material.color.g, material.color.b, 0);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool EarthAttackBehaviour()
    {
        if (!isAttackComplete)
        {
            if (!isInCoroutine)
            {
                StartCoroutine(ChargeEarth());
                isInCoroutine = true;
            }
            return false;
        }
        else
        {
            isAttackComplete = false;
            return true;
        }
        
    }

    IEnumerator ChargeEarth()
    {
        float time = 0f;
        while(time < chillTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0f;
        //MeshRenderer meshRenderer = earthHitBox.GetComponent<MeshRenderer>();
        //Material material = meshRenderer.material;
        //material.color = new Color(material.color.r, material.color.g, material.color.b, 0);
        while(time < 0.6f)
        {
            time += Time.deltaTime;
            //material.color = new Color(material.color.r, material.color.g, material.color.b, maxHitBoxAlpha * time / 1.5f);
            yield return null;

        }
        //material.color = new Color(material.color.r, material.color.g, material.color.b, 0);
        //Activates earthShockwave
        particleSystem.Play();
        earthHitBox.GetComponent<EarthDetectionField>().EarthShockwave(knockUpForce);
        isAttackComplete = true;
        isInCoroutine = false;
    }
}
